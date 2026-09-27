"""Tool dành riêng cho chuyên viên dự báo nhập hàng.

Nguyên tắc chung:
- Mọi tool async, trả về ``str`` (JSON hoặc văn bản tiếng Việt) đúng như
  các tool hiện có trong app/tools.
- Lỗi KHÔNG được ném ra ngoài: trả về thông báo tiếng Việt để LLM biết là
  thiếu dữ liệu và nói ra được, thay vì đoán bừa.
- Số liệu tổng hợp (tổng bán, bình quân, xu hướng, tổng tồn kho) luôn do code
  tính sẵn hoặc do backend .NET tính sẵn, LLM không tự cộng.
"""

import json
from datetime import date, datetime, timezone
from typing import Optional

from langchain_core.tools import tool

from app.config.settings import settings
from app.models.chat import AuthContext
from app.models.forecast import (
    AirQualityDay,
    AirQualityForecast,
    ForecastDay,
    PublicHoliday,
    SalesHistoryResponse,
    SearchTrends,
    TrendKeyword,
    WeatherForecast,
)
from app.services import external_api_client as ext
from app.services.trends_service import get_search_trends
from app.tools.base import api_post, clean_payload, get_field

_TREND_VI = {
    "INCREASING": "TANG",
    "DECREASING": "GIAM",
    "STABLE": "DI NGANG",
}

# Ngưỡng cảnh báo bụi mở theo khuyến cáo WHO (µg/m³)
_PM25_NOTICE = 15.0
_PM25_WARN = 35.0

# /data/2.5/forecast trả 8 mốc/ngày (3 giờ/lần). Ngày đầu chuỗi dự báo thường
# thiếu mốc vì bắt đầu từ thời điểm hiện tại -> bỏ qua để min/max/rain chính xác.
_FORECAST_POINTS_PER_DAY = 8
# Số ngày có dữ liệu đầy đủ: 40 mốc / 8 = 4 ngày trọn vẹn (trừ ngày lương).
_FORECAST_MAX_DAYS = 4
# AQI theo giờ: 24 mốc/ngày. Endpoint này KHÔNG trả timezone nên gom theo ngày
# UTC; lưới mốc cũng căn UTC nên vẫn ra các ngày tròn 24 mốc. Ngày đầu có thể
# thiếu mốc (đã trôi qua trong ngày) nhưng vẫn giữ vì đó là dữ liệu thật của hôm nay.
_AIR_MAX_DAYS = 4

# Tầm nhìn môi trường dùng để thông báo cho LLM (ngày). Cả thời tiết và AQI đều
# bị giới hạn bởi gói API miễn phí nên chỉ phủ được 4 ngày đầu.
ENV_FORECAST_DAYS = min(_FORECAST_MAX_DAYS, _AIR_MAX_DAYS)

# Thứ tự nghiêm trọng của điều kiện thời tiết để chọn "mốc đáng chú ý nhất" trong
# ngày. Số càng lớn càng nghiêm trọng.
_CONDITION_SEVERITY = {
    "tornado": 90, "thunderstorm": 80, "squall": 75, "ash": 70, "sand": 68,
    "dust": 66, "snow": 60, "rain": 50, "drizzle": 40, "mist": 30, "fog": 30,
    "haze": 28, "clouds": 20, "clear": 10,
}

_TREND_KEYWORDS = [
    "triệu chứng đau mắt đỏ",
    "cách trị cúm A",
    "sốt xuất huyết",
    "tay chân miệng",
    "thuốc hạ sốt",
    "bệnh đường hô hấp",
]


def _fail(error: str) -> str:
    return f"Khong lay duoc du lieu: {error}"


def _json(data) -> str:
    return json.dumps(data, ensure_ascii=False, default=str)


def _resolve_location(location: Optional[str]) -> str:
    return (location or "").strip() or settings.DEFAULT_LOCATION


def _dt_to_date(entry: dict, tz_offset: int = 0) -> str:
    """OpenWeather trả 'dt' là unix timestamp -> đổi sang chuỗi yyyy-MM-dd.

    ``tz_offset`` là giây lệch so với UTC (OpenWeather đặt trong ``city.timezone``).
    Dùng giờ địa phương để ngày gom được khớp với "ngày" mà con người hiểu.
    """
    timestamp = get_field(entry, "dt")
    if not timestamp:
        return ""
    try:
        moment = datetime.fromtimestamp(int(timestamp) + int(tz_offset or 0), tz=timezone.utc)
        return moment.date().isoformat()
    except (TypeError, ValueError, OSError):
        return ""


def _mean(values: list[float]) -> Optional[float]:
    return round(sum(values) / len(values), 1) if values else None


def _group_forecast_by_day(payload: dict, days: int) -> list[ForecastDay]:
    """Gom dự báo 3 giờ/lần của /data/2.5/forecast về từng ngày.

    Cấu trúc thật (đã đối chiếu response thật):
    ``{"city": {"timezone": 25200}, "list": [
        {"dt": .., "main": {"temp", "temp_min", "temp_max", "humidity", ..},
         "weather": [{"main": "Clear", "description": "trời quang"}],
         "rain": {"3h": 0.22}, ..}]}``

    Ba điểm dễ sai đã được xử lý ở đây:
    - nhiệt độ và độ ẩm nằm trong ``main``, không phải top-level;
    - ``rain`` là object ``{"3h": mm}`` cho 3 giờ, phải CỘNG lại mới ra tổng ngày;
    - ``main``/``description`` nằm trong ``weather[0]``, không phải top-level.
    """
    tz_offset = get_field(get_field(payload, "city", {}) or {}, "timezone", 0) or 0
    limit = min(max(days, 1), _FORECAST_MAX_DAYS)

    buckets: dict[str, list[dict]] = {}
    for entry in get_field(payload, "list", []) or []:
        if not isinstance(entry, dict):
            continue
        key = _dt_to_date(entry, tz_offset)
        if key:
            buckets.setdefault(key, []).append(entry)

    # Chỉ giữ ngày đủ số mốc TRƯỚC, rồi mới cắt `limit`. Nếu cắt trước thì ngày
    # lương ở đầu sẽ chiếm một suất và kết quả mất đúng một ngày.
    complete_days = [
        day for day in sorted(buckets) if len(buckets[day]) >= _FORECAST_POINTS_PER_DAY
    ][:limit]

    daily: list[ForecastDay] = []
    for day in complete_days:
        entries = buckets[day]

        mains = [get_field(e, "main", {}) or {} for e in entries]

        tmin_values = [m["temp_min"] for m in mains if m.get("temp_min") is not None]
        tmax_values = [m["temp_max"] for m in mains if m.get("temp_max") is not None]
        tmin = round(min(tmin_values), 1) if tmin_values else None
        tmax = round(max(tmax_values), 1) if tmax_values else None
        humidity = _mean([m["humidity"] for m in mains if m.get("humidity") is not None])

        # Cộng mưa của cả ngày: mỗi mốc có tối đa 3 giờ mưa
        rain_total = 0.0
        for entry in entries:
            rain = get_field(entry, "rain", {}) or {}
            try:
                rain_total += float(rain.get("3h", 0) or 0)
            except (AttributeError, TypeError, ValueError):
                continue

        # Chọn mốc "đáng chú ý nhất" để đại diện cho cả ngày
        best_entry, best_rank = None, -1
        for entry in entries:
            weather = get_field(entry, "weather", []) or [{}]
            main = str((get_field(weather[0], "main", "") or "")).strip()
            rank = _CONDITION_SEVERITY.get(main.lower(), 25)
            if rank > best_rank:
                best_entry, best_rank = entry, rank

        weather_list = get_field(best_entry, "weather", []) or [{}] if best_entry else [{}]
        main = str((get_field(weather_list[0], "main", "") or "")).strip()
        description = get_field(weather_list[0], "description")

        daily.append(
            ForecastDay(
                date=day,
                tmin=round(tmin, 1) if tmin is not None else None,
                tmax=round(tmax, 1) if tmax is not None else None,
                humidity=humidity,
                rainMm=round(rain_total, 1),
                condition=ext.condition_vi(main, description),
            )
        )
    return daily


def _group_air_pollution_by_day(entries: list, days: int) -> list[AirQualityDay]:
    """Gom các mốc theo giờ của OpenWeather thành từng ngày.

    Cấu trúc thật (đã đối chiếu response thật):
    ``{"dt": .., "main": {"aqi": 1..5}, "components": {"pm2_5": .., "pm10": .., ..}}``
    ⇒ ``aqi`` nằm trong ``main`` nhưng **bụi nằm trong ``components``**, không
    phải trong ``main``. Đọc nhầm chỗ này khiến PM2.5 luôn ra None.

    API trả 24 mốc/ngày, nên cắt trực tiếp theo `days` sẽ chỉ lấy được vài
    giờ đầu. AQI lấy giá trị xấu nhất trong ngày; PM2.5/PM10 lấy trung bình.
    """
    limit = min(max(days, 1), _AIR_MAX_DAYS)

    buckets: dict[str, list[dict]] = {}
    for entry in entries:
        if not isinstance(entry, dict):
            continue
        key = _dt_to_date(entry)
        if key:
            buckets.setdefault(key, []).append(entry)

    daily: list[AirQualityDay] = []
    for day in sorted(buckets)[:limit]:
        items = buckets[day]
        mains = [get_field(e, "main", {}) or {} for e in items]
        components = [get_field(e, "components", {}) or {} for e in items]

        aqis = [m.get("aqi") for m in mains if m.get("aqi") is not None]
        pm25 = [c["pm2_5"] for c in components if c.get("pm2_5") is not None]
        pm10 = [c["pm10"] for c in components if c.get("pm10") is not None]
        worst = max(aqis) if aqis else None
        daily.append(
            AirQualityDay(
                date=day,
                aqi=worst,
                pm2_5=round(sum(pm25) / len(pm25), 1) if pm25 else None,
                pm10=round(sum(pm10) / len(pm10), 1) if pm10 else None,
                level=ext.aqi_level_vi(worst),
            )
        )
    return daily


def create_forecast_tools(auth: AuthContext):
    # ------------------------------------------------------- dữ liệu nội bộ

    @tool
    async def get_sales_history(
        keyword: Optional[str] = None,
        medicine_id: Optional[int] = None,
        days: int = 30,
    ) -> str:
        """
        LICH SU BAN HANG THEO THUOC - duyet dung cho agent du bao nhap hang.
        Backend .NET da tinh san tong, binh quan va xu huong, ban KHONG can tu cong.

        Args:
            keyword: Ten thuoc can tra cuu (dung khi khong biet ma)
            medicine_id: ID thuoc (uu tien hon keyword neu biet)
            days: So ngay nhin lai, mac dinh 30, toi da 90

        Ket qua gom: tong ban trong ky, binh quan moi ngay, xu hong
        (INCREASING/DECREASING/STABLE) va ban ghi tung ngay.
        Chi nhanh lay tu header cua phien dang dang nhap.
        """
        if not keyword and not medicine_id:
            return "Can cung cap keyword (ten thuoc) hoac medicine_id."

        payload = clean_payload({
            "MedicineId": medicine_id,
            "Keyword": keyword,
            "Days": days,
        })
        data = await api_post(
            "/api/ai/sales/history",
            json_body=payload,
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return _fail(data["error"])

        try:
            history = SalesHistoryResponse.model_validate(data)
        except Exception as e:  # noqa: BLE001 - payload khong khop schema
            return _fail(f"du lieu lich su ban khong dung dinh dang ({e})")

        unit = f" {history.unitName}" if history.unitName else ""
        trend = _TREND_VI.get(
            (history.summary.salesTrend or "").upper(),
            history.summary.salesTrend,
        )
        return (
            f"{history.productName} ({history.unitName or 'khong ro don vi'}), "
            f"{history.periodDays} ngay qua: "
            f"tong ban {history.summary.totalQuantitySold}{unit}, "
            f"binh quan {history.summary.averageDaily}/ngay, "
            f"xu huong he thong danh gia: {trend}. "
            f"Chi tiet tung ngay: {history.dailyRecords}"
        )

    @tool
    async def get_product_stock(
        keyword: Optional[str] = None,
        medicine_id: Optional[int] = None,
    ) -> str:
        """
        TON KHO THUOC - tong so luong con ton tai tat ca cac lo thuoc,
        da duoc code tong hop san (ban KHONG can tu cong cac dong).

        Args:
            keyword: Ten thuoc can tra cuu
            medicine_id: ID thuoc (uu tien hon keyword neu biet)

        Ket qua gom: tong ton, so lo, han su dung som nhat, don vi.
        """
        if not keyword and not medicine_id:
            return "Can cung cap keyword (ten thuoc) hoac medicine_id."

        payload = clean_payload({
            "Keyword": keyword,
            "MedicineId": medicine_id,
            "Count": 200,
        })
        data = await api_post(
            "/api/ai/inventory/search",
            json_body=payload,
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return _fail(data["error"])

        items = get_field(data, "Items", []) if isinstance(data, dict) else (data or [])
        if not items:
            return "Khong tim thay thuoc nay trong ton kho."

        # Gop theo MedicineID de moi thuoc chi xuat hien mot lan
        grouped: dict = {}
        for row in items:
            if not isinstance(row, dict):
                continue
            medicine_key = get_field(row, "MedicineId")
            if medicine_key not in grouped:
                grouped[medicine_key] = {
                    "medicineId": medicine_key,
                    "medicineName": get_field(row, "MedicineName"),
                    "unitName": get_field(row, "UnitName"),
                    "totalInStock": 0.0,
                    "batchCount": 0,
                    "earliestExpiry": None,
                }
            bucket = grouped[medicine_key]
            try:
                bucket["totalInStock"] += float(get_field(row, "QuantityInStock", 0) or 0)
            except (TypeError, ValueError):
                pass
            bucket["batchCount"] += 1
            expiry = get_field(row, "ExpiryDate")
            if expiry and (bucket["earliestExpiry"] is None or str(expiry) < bucket["earliestExpiry"]):
                bucket["earliestExpiry"] = str(expiry)

        if not grouped:
            return "Khong tim thay thuoc nay trong ton kho."

        if len(grouped) > 1:
            names = ", ".join(str(g["medicineName"]) for g in grouped.values())
            return (
                f"Tu khoa khop nhieu thuoc ({len(grouped)}): {names}. "
                "Hay chi dinh ten thuoc chinh xac hon roi goi lai."
            )

        return _json(next(iter(grouped.values())))

    # ------------------------------------------------------- thoi tiet / AQI

    @tool
    async def get_weather_forecast(location: Optional[str] = None, days: int = 4) -> str:
        """
        DU BAO THOI TIET cho mot tinh/thanh, phuc vu du bao nhu cau thuoc.

        Args:
            location: Ten tinh/thanh, vi du "Ha Noi". Bo trong se dung mac dinh.
            days: So ngay, mac dinh 4, toi da 4 (API chi du 5 ngay va 4 ngay
                  co du so moc 3 gio/day)

        Tac dong: ret lan, mua ret, nam tang -> tang nhu cau thuoc cam cum,
        thuoc ho, thuoc nho mau, vitamin. Nong lon -> tang thuoc lam duong mat.
        """
        target = _resolve_location(location)
        if not settings.openweather_enabled:
            return "Khong co OPENWEATHER_API_KEY nen khong lay duoc du bao thoi tiet."

        try:
            payload = await ext.get_weather_forecast(target, days=min(max(days, 1), _FORECAST_MAX_DAYS))
        except Exception as e:  # noqa: BLE001
            return _fail(f"thoi tiet cho '{target}' ({e})")

        if not payload:
            return _fail(f"khong tim thay dia diem '{target}' hoac API tra rong.")

        try:
            daily = _group_forecast_by_day(payload, days)
        except Exception as e:  # noqa: BLE001 - schema tra ve khong dung thi giu sao
            return _fail(f"du lieu thoi tiet khong dung dinh dang ({e})")

        if not daily:
            return _fail("API khong tra ve ngay nao du so du lieu 3 gio.")

        forecast = WeatherForecast(
            location=target,
            summary=_summarize_weather(daily),
            daily=daily,
        )
        return _json(forecast.model_dump())

    @tool
    async def get_air_quality(location: Optional[str] = None, days: int = 4) -> str:
        """
        CHAT LUONG KHONG KHI (AQI, PM2.5, PM10) cho mot tinh/thanh.

        Args:
            location: Ten tinh/thanh, vi du "Ha Noi". Bo trong se dung mac dinh.
            days: So ngay, mac dinh 4, toi da 4 (API du 4 ngay theo gio)

        Tac dong khi PM2.5 tang cao: khau trang y te, nuoc muoi sinh ly
        (rua mui, suc hong), thuoc nho mat nhan tao va thuoc dan phe quan
        cho nguoi hen suyen deu tang manh.
        """
        target = _resolve_location(location)
        if not settings.openweather_enabled:
            return "Khong co OPENWEATHER_API_KEY nen khong lay duoc chi so AQI."

        try:
            payload = await ext.get_air_pollution_forecast(target, days=min(max(days, 1), _AIR_MAX_DAYS))
        except Exception as e:  # noqa: BLE001
            return _fail(f"chat luong khong khi cho '{target}' ({e})")

        if not payload:
            return _fail(f"khong tim thay dia diem '{target}' hoac API tra rong.")

        try:
            daily = _group_air_pollution_by_day(payload.get("list") or [], min(max(days, 1), _AIR_MAX_DAYS))
        except Exception as e:  # noqa: BLE001 - schema tra ve khong dung thi giu sao
            return _fail(f"du lieu chat luong khong khi khong dung dinh dang ({e})")

        if not daily:
            return _fail("API khong tra ve du lieu chat luong khong khi.")

        forecast = AirQualityForecast(
            location=target,
            summary=_summarize_air(daily),
            daily=daily,
        )
        return _json(forecast.model_dump())

    # -------------------------------------------------- dich benh / lich le

    @tool
    async def get_search_trends_tool(
        keywords: Optional[str] = None,
        location: Optional[str] = None,
        days: int = 7,
    ) -> str:
        """
        XU HUONG TIM KIEM GOOGLE TRENDS cho cac tu khoa benh phong tai mot tinh/thanh.
        Xu huong tim kiem thuong di truoc nhu cau mua thuoc 2-3 ngay.

        Args:
            keywords: Danh sach tu khoa nganh cach nhau bang dau phay.
                      Bo trong se dung bo khoa mac dinh ve benh mua tai.
            location: Ten tinh/thanh, vi du "Ha Noi"
            days: So ngay, mac dinh 7

        Ket qua: muc do quan tam trung binh, xu huong tang/giam/di ngang,
        va chuoi diem theo ngay cho tung tu khoa.
        """
        target = _resolve_location(location)
        if keywords:
            key_list = [k.strip() for k in keywords.split(",") if k.strip()]
        else:
            key_list = list(_TREND_KEYWORDS)

        result = await get_search_trends(key_list, target, days=days)
        if not result:
            return (
                "Khong lay duoc xu huong Google Trends (bi gioi han tan suat "
                "hoac dich vu dang tam). Hay dua vao du lieu ban/tinh kho va thoi tiet."
            )

        trends = SearchTrends(
            location=target,
            days=days,
            keywords=[
                TrendKeyword(
                    keyword=name,
                    avgInterest=payload["avg"],
                    slope=payload["slope"],
                    timeline=payload["timeline"],
                )
                for name, payload in result.items()
            ],
        )
        return _json(trends.model_dump())

    @tool
    async def get_holidays(country_code: str = "VN", days_ahead: int = 30) -> str:
        """
        LICH NGHI LE sap toi can nhac truoc khi du bao nhap hang.

        Args:
            country_code: Ma nuoc, mac dinh VN
            days_ahead: So ngay nhin truoc, mac dinh 30, toi da 180

        Tac dong:
        - Le/Tet: tang dot bien thuoc ho tro tieu hoa, giai ruou, bao ve gan.
        - Mua du lich: thuoc say tau xe, thuoc tieu chay cap, bang gac ca nhan,
          kem chong muoi.
        """
        code = (country_code or "VN").upper()
        today = date.today()
        horizon = today.fromordinal(today.toordinal() + min(max(days_ahead, 1), 180))

        try:
            raw = await ext.get_public_holidays(code, today.year)
            # Ngày lễ có thể trải sang năm sau nên cần tra thêm năm kế tiếp
            if horizon.year > today.year:
                raw = raw + await ext.get_public_holidays(code, horizon.year)
        except Exception as e:  # noqa: BLE001
            return _fail(f"lich nghi le '{code}' ({e})")

        upcoming: list[PublicHoliday] = []
        for item in raw or []:
            if not isinstance(item, dict):
                continue
            raw_date = get_field(item, "date")
            if not raw_date:
                continue
            try:
                event = date.fromisoformat(str(raw_date)[:10])
            except ValueError:
                continue
            if today <= event <= horizon:
                upcoming.append(
                    PublicHoliday(
                        date=event.isoformat(),
                        name=str(get_field(item, "name", "")),
                        localName=get_field(item, "localName"),
                        daysUntil=(event - today).days,
                    )
                )

        if not upcoming:
            return f"Khong co le lon nao cua '{code}' trong {min(max(days_ahead, 1), 180)} ngay toi."

        upcoming.sort(key=lambda h: h.date)
        return _json({"countryCode": code, "holidays": [h.model_dump() for h in upcoming]})

    return [
        get_sales_history,
        get_product_stock,
        get_weather_forecast,
        get_air_quality,
        get_search_trends_tool,
        get_holidays,
    ]


# ------------------------------------------------------------------ tom tat

def _summarize_weather(daily: list[ForecastDay]) -> str:
    if not daily:
        return "Không có dự báo thời tiết."

    tmin = min((d.tmin for d in daily if d.tmin is not None), default=None)
    tmax = max((d.tmax for d in daily if d.tmax is not None), default=None)
    rain_days = sum(1 for d in daily if (d.rainMm or 0) > 0)
    total_rain = sum(d.rainMm or 0 for d in daily)
    coldest = min(
        (d for d in daily if d.tmin is not None),
        key=lambda d: d.tmin,
        default=None,
    )

    parts = []
    if tmin is not None and tmax is not None:
        parts.append(f"Nhiệt độ {tmin:.0f}-{tmax:.0f} độ C")
    if coldest is not None and tmin is not None and tmin <= 15:
        parts.append(f" rét mạnh vào ngày {coldest.date}")
    if rain_days:
        parts.append(f"mưa {rain_days}/{len(daily)} ngày, tổng {total_rain:.1f} mm")
    else:
        parts.append("không có mưa")
    return "; ".join(parts) + "."


def _summarize_air(daily: list[AirQualityDay]) -> str:
    if not daily:
        return "Không có dữ liệu chất lượng không khí."

    pm25 = [d.pm2_5 for d in daily if d.pm2_5 is not None]
    if not pm25:
        return "Không có số liệu bụi mịn PM2.5."

    peak = max(pm25)
    parts = [f"PM2.5 trung bình {sum(pm25) / len(pm25):.1f} µg/m³"]
    if peak >= _PM25_WARN:
        parts.append(" có ngày PM2.5 vượt ngưỡng báo hại (35 µg/m³), khuyến nghị tăng nhập khẩu trang và nước muối")
    elif peak >= _PM25_NOTICE:
        parts.append(" PM2.5 vượt ngưỡng cảnh báo WHO (15 µg/m³), dự kiến tăng nhu cầu bảo vệ hô hấp")
    else:
        parts.append(" PM2.5 dưới ngưỡng cảnh báo, ảnh hưởng nhu cầu ở mức thông thường")
    return "; ".join(parts) + "."
