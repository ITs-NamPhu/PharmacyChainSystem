import json
from datetime import datetime, timezone

import pytest

from app.config.settings import settings
from app.services import external_api_client as ext
from app.services.trends_service import _slope
from app.tools.forecast_tools import create_forecast_tools


@pytest.fixture
def ftools(auth):
    return {t.name: t for t in create_forecast_tools(auth)}


@pytest.fixture(autouse=True)
def _clear_external_cache():
    ext.clear_cache()
    yield
    ext.clear_cache()


def _last_call(backend_stub):
    assert backend_stub.calls, "no backend call recorded"
    return backend_stub.calls[-1]


# ------------------------------------------------------------ sales history

_SAMPLE_HISTORY = {
    "productId": 12,
    "productName": "Thuoc cam cum Tiffy",
    "branchId": 1,
    "periodDays": 30,
    "unitName": "vỉ",
    "summary": {
        "totalQuantitySold": 150.0,
        "averageDaily": 5.0,
        "salesTrend": "INCREASING",
    },
    "dailyRecords": {"2026-09-20": 4, "2026-09-21": 25, "2026-09-22": 0},
}


@pytest.mark.asyncio
async def test_get_sales_history_happy_path(ftools, backend_stub):
    backend_stub.set_response("/api/ai/sales/history", _SAMPLE_HISTORY)
    out = await ftools["get_sales_history"].ainvoke(
        {"keyword": "tiffy", "days": 30}
    )
    call = _last_call(backend_stub)
    assert call["path"] == "/api/ai/sales/history"
    assert call["json_body"] == {"Keyword": "tiffy", "Days": 30}
    assert call["token"] == "test-token"
    assert call["branch_id"] == "1"
    # Chi nhánh lay tu header nen khong gui BranchId trong body
    assert "BranchId" not in call["json_body"]
    assert "150" in out
    assert "5.0" in out
    assert "TANG" in out


@pytest.mark.asyncio
async def test_get_sales_history_by_medicine_id(ftools, backend_stub):
    backend_stub.set_response("/api/ai/sales/history", _SAMPLE_HISTORY)
    await ftools["get_sales_history"].ainvoke({"medicine_id": 12})
    assert _last_call(backend_stub)["json_body"] == {"MedicineId": 12, "Days": 30}


@pytest.mark.asyncio
async def test_get_sales_history_requires_keyword_or_id(ftools, backend_stub):
    out = await ftools["get_sales_history"].ainvoke({})
    assert "Can cung cap" in out
    assert backend_stub.calls == []


@pytest.mark.asyncio
async def test_get_sales_history_backend_error(ftools, backend_stub):
    backend_stub.set_response(
        "/api/ai/sales/history",
        {"error": "Tu khoa 'tiffy' khop nhieu thuoc: A, B"},
    )
    out = await ftools["get_sales_history"].ainvoke({"keyword": "tiffy"})
    assert "Khong lay duoc du lieu" in out
    assert "khop nhieu thuoc" in out


@pytest.mark.asyncio
async def test_get_sales_history_invalid_schema(ftools, backend_stub):
    backend_stub.set_response("/api/ai/sales/history", {"unexpected": True})
    out = await ftools["get_sales_history"].ainvoke({"keyword": "tiffy"})
    assert "khong dung dinh dang" in out


# ------------------------------------------------------------- product stock

@pytest.mark.asyncio
async def test_get_product_stock_aggregates_batches(ftools, backend_stub):
    backend_stub.set_response(
        "/api/ai/inventory/search",
        {
            "Items": [
                {"MedicineId": 12, "MedicineName": "Tiffy", "UnitName": "vỉ",
                 "QuantityInStock": 10, "ExpiryDate": "2027-01-01"},
                {"MedicineId": 12, "MedicineName": "Tiffy", "UnitName": "vỉ",
                 "QuantityInStock": 5, "ExpiryDate": "2026-12-01"},
            ]
        },
    )
    out = await ftools["get_product_stock"].ainvoke({"keyword": "tiffy"})
    data = json.loads(out)
    assert data["medicineId"] == 12
    assert data["totalInStock"] == 15.0
    assert data["batchCount"] == 2
    assert data["earliestExpiry"] == "2026-12-01"
    assert _last_call(backend_stub)["path"] == "/api/ai/inventory/search"


@pytest.mark.asyncio
async def test_get_product_stock_asks_when_ambiguous(ftools, backend_stub):
    backend_stub.set_response(
        "/api/ai/inventory/search",
        {
            "Items": [
                {"MedicineId": 1, "MedicineName": "Tiffy A", "QuantityInStock": 3},
                {"MedicineId": 2, "MedicineName": "Tiffy B", "QuantityInStock": 4},
            ]
        },
    )
    out = await ftools["get_product_stock"].ainvoke({"keyword": "tiffy"})
    assert "khop nhieu thuoc" in out
    assert "Tiffy A" in out and "Tiffy B" in out


@pytest.mark.asyncio
async def test_get_product_stock_not_found(ftools, backend_stub):
    backend_stub.set_response("/api/ai/inventory/search", {"Items": []})
    out = await ftools["get_product_stock"].ainvoke({"keyword": "khong-ton-tai"})
    assert "Khong tim thay thuoc" in out


# ------------------------------------------------------------- weather / AQI
#
# Fixture bên dưới sao chép NGUYÊN VĂN cấu trúc response thật đã lấy từ
# OpenWeather (đã kiểm tra bằng cách gọi API thật, không phải suy đoán):
#
#   GET /data/2.5/forecast
#     {"city": {"timezone": 25200}, "list": [
#        {"dt": ..,
#         "main": {"temp":.., "temp_min":.., "temp_max":.., "humidity":..},
#         "weather": [{"main": "Clear", "description": "trời quang"}],
#         "rain": {"3h": 0.22}}]}
#
#   GET /data/2.5/air_pollution/forecast
#     {"coord": {...}, "list": [
#        {"dt": .., "main": {"aqi": 1..5},
#         "components": {"pm2_5":.., "pm10":.., "co":.., ...}}]}
#
# Ba điểm từng gây bug: temp/humidity nằm trong `main`; `rain` là {"3h": mm};
# `main`/`description` của thời tiết nằm trong `weather[0]`; PM2.5 nằm trong
# `components` chứ không phải `main`.

_TZ_VN = 25200  # UTC+7, giá trị `city.timezone` thật của Việt Nam


def _local_midnight(year, month, day, tz_offset):
    """Unix timestamp của NỬA ĐÊM GIỜ ĐỊA PHƯƠNG (vd giờ VN) quy ra UTC.

    Dùng datetime để không phải đoán số epoch bằng tay -- tính sai một ngày là
    fixture vẫn chạy nhưng assert ngày sai và che mất bug về múi giờ.
    """
    return int(datetime(year, month, day, tzinfo=timezone.utc).timestamp()) - tz_offset


# OpenWeather căn chuỗi dự báo 3 giờ theo NỬA ĐÊM GIỜ ĐỊA PHƯƠNG, không phải UTC.
# Với VN (UTC+7) nửa đêm 2026-09-14 giờ VN = 2026-09-13 17:00 UTC.
_DAY1 = _local_midnight(2026, 9, 14, _TZ_VN)
_DAY2 = _local_midnight(2026, 9, 15, _TZ_VN)

# Riêng AQI: endpoint không trả timezone và lưới mốc CĂN THEO UTC (đã đối chiếu
# response thật: chuỗi bắt đầu lúc 14:00 UTC), nên ngày AQI tính theo UTC.
_AQI_DAY1 = _local_midnight(2026, 9, 14, 0)
_AQI_DAY2 = _local_midnight(2026, 9, 15, 0)


def _fc_point(dt, tmin, tmax, humidity, cond="Clear", desc="trời quang", rain=None):
    """Dựng 1 mốc 3 giờ đúng shape /data/2.5/forecast."""
    point = {
        "dt": dt,
        "main": {
            "temp": round((tmin + tmax) / 2, 2),
            "temp_min": tmin,
            "temp_max": tmax,
            "humidity": humidity,
            "pressure": 1012,
        },
        "weather": [{"id": 800, "main": cond, "description": desc, "icon": "01d"}],
        "pop": 0.0,
        "clouds": 0,
        "wind": {"speed": 3.2, "deg": 90},
    }
    if rain is not None:
        point["rain"] = {"3h": rain}
    return point


def _fc_day(base_utc, tmin, tmax, humidity, cond="Clear", desc="trời quang", rains=None):
    """8 mốc 3 giờ = 1 ngày đầy đủ. `base_utc` phải là NỬA ĐÊM GIỜ ĐỊA PHƯƠNG
    quy ra UTC, nếu không 8 mốc sẽ tràn sang ngày kế tiếp (đúng như API thật)."""
    points = []
    for i in range(8):
        rain = None if rains is None else rains[i]
        points.append(
            _fc_point(base_utc + i * 10800, tmin, tmax, humidity, cond, desc, rain)
        )
    return points


def _aqi_point(dt, aqi, pm25, pm10):
    """Dựng 1 mốc theo giờ đúng shape air_pollution/forecast."""
    return {
        "dt": dt,
        "main": {"aqi": aqi},
        "components": {
            "co": 201.94, "no": 0.02, "no2": 4.1, "o3": 68.6,
            "so2": 1.2, "nh3": 1.1, "pm2_5": pm25, "pm10": pm10,
        },
    }


def _aqi_day(base_utc, aqi, pm25, pm10):
    """24 mốc/giờ = 1 ngày UTC đầy đủ (AQI dùng mốc UTC, xem `_AQI_DAY1`)."""
    return [_aqi_point(base_utc + i * 3600, aqi, pm25, pm10) for i in range(24)]


def _mock_geo(mock_http):
    mock_http.get(url__startswith="https://api.openweathermap.org/geo/1.0/direct").respond(
        200, json=[{"name": "Xom Pho", "lat": 21.0283334, "lon": 105.854041, "country": "VN"}]
    )


@pytest.mark.asyncio
async def test_get_weather_forecast_without_api_key(ftools, monkeypatch):
    monkeypatch.setattr(settings, "OPENWEATHER_API_KEY", None)
    out = await ftools["get_weather_forecast"].ainvoke({"location": "Ha Noi"})
    assert "OPENWEATHER_API_KEY" in out


@pytest.mark.asyncio
async def test_get_weather_forecast_groups_three_hourly_points_by_day(ftools, monkeypatch, mock_http):
    """8 moc 3 gio/ngay -> phai ra 1 dong/ngay, dung shape that cua 2.5/forecast."""
    monkeypatch.setattr(settings, "OPENWEATHER_API_KEY", "test-key")
    _mock_geo(mock_http)
    mock_http.get(url__startswith="https://api.openweathermap.org/data/2.5/forecast").respond(
        200,
        json={
            "cod": "200",
            "cnt": 16,
            "city": {"name": "Xom Pho", "country": "VN", "timezone": _TZ_VN},
            "list": (
                # Ngày 1: 8 mốc, có mưa 2 khoảng
                _fc_day(_DAY1, 12.0, 18.0, 85, "Rain", "mưa rào", rains=[1.2, 0, 0, 0, 3.4, 0, 0, 0])
                # Ngày 2: 8 mốc, trời quang
                + _fc_day(_DAY2, 11.0, 16.0, 90, "Clear", "trời quang")
            ),
        },
    )
    out = await ftools["get_weather_forecast"].ainvoke({"location": "Ha Noi", "days": 2})
    data = json.loads(out)
    assert len(data["daily"]) == 2

    day1 = data["daily"][0]
    # temp/humidity phải lấy từ `main`
    assert day1["tmin"] == 12.0
    assert day1["tmax"] == 18.0
    assert day1["humidity"] == 85.0
    # `rain` là {"3h": mm} -> phải CỘNG lại thành tổng ngày (1.2 + 3.4 = 4.6)
    assert day1["rainMm"] == 4.6
    # `main`/description của thời tiết nằm trong weather[0]
    assert day1["condition"] == "mưa"

    day2 = data["daily"][0 + 1]
    assert day2["tmin"] == 11.0
    assert day2["rainMm"] == 0.0
    assert day2["condition"] == "trời quang"

    assert "11-18 độ C" in data["summary"]
    assert "mưa 1/2 ngày" in data["summary"]


@pytest.mark.asyncio
async def test_get_weather_forecast_picks_most_severe_condition_of_the_day(ftools, monkeypatch, mock_http):
    """Trong ngày có cả Clear lẫn Thunderstorm thì phải lấy điều kiện nặng nhất."""
    monkeypatch.setattr(settings, "OPENWEATHER_API_KEY", "test-key")
    _mock_geo(mock_http)
    points = _fc_day(_DAY1, 20.0, 26.0, 70, "Clear", "trời quang")
    points[3] = _fc_point(_DAY1 + 3 * 10800, 20.0, 26.0, 70, "Thunderstorm", "giông")
    points[5] = _fc_point(_DAY1 + 5 * 10800, 20.0, 26.0, 70, "Rain", "mưa")
    mock_http.get(url__startswith="https://api.openweathermap.org/data/2.5/forecast").respond(
        200,
        json={"cod": "200", "city": {"timezone": _TZ_VN}, "list": points},
    )
    out = await ftools["get_weather_forecast"].ainvoke({"location": "Ha Noi", "days": 1})
    data = json.loads(out)
    assert data["daily"][0]["condition"] == "giông"


@pytest.mark.asyncio
async def test_get_weather_forecast_skips_partial_first_day(ftools, monkeypatch, mock_http):
    """Ngày đầu chuỗi dự báo thiếu mốc -> phải bỏ qua, không tính min/max từ 1 mốc."""
    monkeypatch.setattr(settings, "OPENWEATHER_API_KEY", "test-key")
    _mock_geo(mock_http)
    partial = _fc_day(_DAY1, 30.0, 34.0, 60)[:1]  # chỉ 1 mốc
    full = _fc_day(_DAY2, 12.0, 18.0, 85, "Rain", "mưa", rains=[2.0, 0, 0, 0, 0, 0, 0, 0])
    mock_http.get(url__startswith="https://api.openweathermap.org/data/2.5/forecast").respond(
        200,
        json={"cod": "200", "city": {"timezone": _TZ_VN}, "list": partial + full},
    )
    out = await ftools["get_weather_forecast"].ainvoke({"location": "Ha Noi", "days": 2})
    data = json.loads(out)
    assert len(data["daily"]) == 1
    assert data["daily"][0]["tmin"] == 12.0  # không phải 30.0 của ngày lương
    assert data["daily"][0]["tmax"] == 18.0


@pytest.mark.asyncio
async def test_get_weather_forecast_uses_city_timezone_to_group(ftools, monkeypatch, mock_http):
    """Cùng một chuỗi mốc, nhưng kết quả gom ngày phải đổi theo `city.timezone`.

    Chuỗi 8 mốc bắt đầu lúc 17:00 UTC. Với UTC+7 cả 8 mốc rơi vào 2026-09-14
    (nửa đêm giờ VN) -> 1 ngày đầy đủ. Nếu code bỏ qua `city.timezone` và gom theo
    UTC, chuỗi đó sẽ bị cắt làm hai ngày lương và không còn ngày nào đủ 8 mốc.
    """
    monkeypatch.setattr(settings, "OPENWEATHER_API_KEY", "test-key")
    _mock_geo(mock_http)
    base = _local_midnight(2026, 9, 14, 0) - 7 * 3600  # 17:00 UTC của 2026-09-13
    points = _fc_day(base, 12.0, 18.0, 85)
    route = mock_http.get(
        url__startswith="https://api.openweathermap.org/data/2.5/forecast"
    )

    route.respond(200, json={"cod": "200", "city": {"timezone": _TZ_VN}, "list": points})
    out = await ftools["get_weather_forecast"].ainvoke({"location": "Ha Noi", "days": 1})
    data = json.loads(out)
    assert len(data["daily"]) == 1
    assert data["daily"][0]["date"] == "2026-09-14"

    # Cùng payload nhưng timezone = 0: 8 mốc bị chia đôi, không còn ngày nào đủ
    ext.clear_cache()  # client cache theo lat/lon, không clear thì trả kết quả cũ
    route.respond(200, json={"cod": "200", "city": {"timezone": 0}, "list": points})
    out = await ftools["get_weather_forecast"].ainvoke({"location": "Ha Noi", "days": 1})
    assert "Khong lay duoc du lieu" in out


@pytest.mark.asyncio
async def test_get_weather_forecast_real_api_shape_yields_four_full_days(ftools, monkeypatch, mock_http):
    """Tái hiện đúng chuỗi 40 mốc của API thật: 1 + 8 + 8 + 8 + 8 + 7.

    Chuỗi thật luôn bắt đầu lúc "bây giờ" nên ngày đầu thiếu mốc và ngày cuối
    cũng thiếu. Phải loại 2 ngày lương đó rồi mới lấy 4 ngày đầy đủ.

    Regression: cắt `[:limit]` TRƯỚC khi lọc ngày lương sẽ mất một ngày và chỉ
    còn 3 ngày.
    """
    monkeypatch.setattr(settings, "OPENWEATHER_API_KEY", "test-key")
    _mock_geo(mock_http)
    real_shape = (
        _fc_day(_DAY1, 28.0, 32.0, 75)[:1]  # hôm nay, mới có 1 mốc
        + _fc_day(_DAY2, 26.0, 30.0, 80)
        + _fc_day(_local_midnight(2026, 9, 16, _TZ_VN), 25.0, 29.0, 82)
        + _fc_day(_local_midnight(2026, 9, 17, _TZ_VN), 24.0, 28.0, 85)
        + _fc_day(_local_midnight(2026, 9, 18, _TZ_VN), 23.0, 27.0, 88)
        + _fc_day(_local_midnight(2026, 9, 19, _TZ_VN), 22.0, 26.0, 90)[:7]  # ngày cuối lương
    )
    assert len(real_shape) == 40  # đúng bằng `cnt` của API thật
    mock_http.get(url__startswith="https://api.openweathermap.org/data/2.5/forecast").respond(
        200,
        json={"cod": "200", "cnt": 40, "city": {"timezone": _TZ_VN}, "list": real_shape},
    )
    out = await ftools["get_weather_forecast"].ainvoke({"location": "Ha Noi", "days": 4})
    data = json.loads(out)
    assert [d["date"] for d in data["daily"]] == [
        "2026-09-15", "2026-09-16", "2026-09-17", "2026-09-18",
    ]
    assert data["daily"][0]["tmin"] == 26.0  # không phải 28.0 của ngày lương hôm nay


@pytest.mark.asyncio
async def test_get_weather_forecast_caps_days_at_four(ftools, monkeypatch, mock_http):
    """Gói miễn phí chỉ có 4 ngày đầy đủ -> days=30 vẫn trả về tối đa 4."""
    monkeypatch.setattr(settings, "OPENWEATHER_API_KEY", "test-key")
    _mock_geo(mock_http)
    days_points = []
    for d in range(6):
        days_points += _fc_day(_DAY1 + d * 86400, 12.0, 18.0, 85)
    mock_http.get(url__startswith="https://api.openweathermap.org/data/2.5/forecast").respond(
        200,
        json={"cod": "200", "city": {"timezone": _TZ_VN}, "list": days_points},
    )
    out = await ftools["get_weather_forecast"].ainvoke({"location": "Ha Noi", "days": 30})
    data = json.loads(out)
    assert len(data["daily"]) == 4


@pytest.mark.asyncio
async def test_get_weather_forecast_survives_bad_schema(ftools, monkeypatch, mock_http):
    """Schema lạ phải trả về thông báo chứ không ném exception ra ngoài."""
    monkeypatch.setattr(settings, "OPENWEATHER_API_KEY", "test-key")
    _mock_geo(mock_http)
    mock_http.get(url__startswith="https://api.openweathermap.org/data/2.5/forecast").respond(
        200, json={"cod": "200", "city": {"timezone": _TZ_VN}, "list": [{"dt": "abc"}] * 8}
    )
    out = await ftools["get_weather_forecast"].ainvoke({"location": "Ha Noi"})
    # hoặc báo lỗi schema, hoặc không có ngày hợp lệ nào -> đều là string, không raise
    assert isinstance(out, str)
    assert "Khong lay duoc du lieu" in out or "du lieu" in out


@pytest.mark.asyncio
async def test_get_weather_forecast_unknown_location(ftools, monkeypatch, mock_http):
    monkeypatch.setattr(settings, "OPENWEATHER_API_KEY", "test-key")
    mock_http.get(url__startswith="https://api.openweathermap.org/geo/1.0/direct").respond(
        200, json=[]
    )
    out = await ftools["get_weather_forecast"].ainvoke({"location": "Khong Ton Tai"})
    assert "Khong lay duoc du lieu" in out


@pytest.mark.asyncio
async def test_get_air_quality_happy_path(ftools, monkeypatch, mock_http):
    monkeypatch.setattr(settings, "OPENWEATHER_API_KEY", "test-key")
    _mock_geo(mock_http)
    mock_http.get(
        url__startswith="https://api.openweathermap.org/data/2.5/air_pollution/forecast"
    ).respond(
        200,
        json={
            "coord": {"lon": 105.854041, "lat": 21.0283334},
            "list": _aqi_day(_AQI_DAY1, 4, 48.3, 80.0),
        },
    )
    out = await ftools["get_air_quality"].ainvoke({"location": "Ha Noi", "days": 1})
    data = json.loads(out)
    # OpenWeather dùng thang AQI 1-5, không phải 1-500
    assert data["daily"][0]["aqi"] == 4
    assert data["daily"][0]["level"] == "xấu"
    # PM2.5 nằm trong `components`, KHÔNG nằm trong `main`
    assert data["daily"][0]["pm2_5"] == 48.3
    assert data["daily"][0]["pm10"] == 80.0
    assert "vượt ngưỡng báo hại" in data["summary"]


@pytest.mark.asyncio
async def test_get_air_quality_reads_pm_from_components_not_main(ftools, monkeypatch, mock_http):
    """Regression: PM2.5 phải đọc từ `components`.

    Nếu code đọc nhầm trong `main`, pm2_5 sẽ là None và cả phần "tăng nhu cầu
    khẩu trang/nước muối" trong báo cáo dự báo sẽ chết.
    """
    monkeypatch.setattr(settings, "OPENWEATHER_API_KEY", "test-key")
    _mock_geo(mock_http)
    mock_http.get(
        url__startswith="https://api.openweathermap.org/data/2.5/air_pollution/forecast"
    ).respond(
        200,
        json={"coord": {}, "list": _aqi_day(_AQI_DAY1, 3, 20.0, 40.0)},
    )
    out = await ftools["get_air_quality"].ainvoke({"location": "Ha Noi", "days": 1})
    data = json.loads(out)
    assert data["daily"][0]["pm2_5"] is not None
    assert data["daily"][0]["pm2_5"] == 20.0
    # 20 µg/m³ nằm giữa ngưỡng cảnh báo (15) và báo hại (35)
    assert "vượt ngưỡng cảnh báo WHO" in data["summary"]


@pytest.mark.asyncio
async def test_get_air_quality_aggregates_hourly_points_by_day(ftools, monkeypatch, mock_http):
    """24 mốc/ngày theo giờ -> AQI lấy max, PM lấy trung bình."""
    monkeypatch.setattr(settings, "OPENWEATHER_API_KEY", "test-key")
    _mock_geo(mock_http)
    day1 = [_aqi_point(_AQI_DAY1 + i * 3600, 2, 10.0, 20.0) for i in range(12)]
    day1 += [_aqi_point(_AQI_DAY1 + i * 3600, 3, 20.0, 30.0) for i in range(12)]
    day2 = [_aqi_point(_AQI_DAY2 + i * 3600, 5, 60.0, 70.0) for i in range(24)]
    mock_http.get(
        url__startswith="https://api.openweathermap.org/data/2.5/air_pollution/forecast"
    ).respond(
        200, json={"coord": {}, "list": day1 + day2}
    )
    out = await ftools["get_air_quality"].ainvoke({"location": "Ha Noi", "days": 2})
    data = json.loads(out)
    assert len(data["daily"]) == 2
    assert data["daily"][0]["aqi"] == 3
    assert data["daily"][0]["pm2_5"] == 15.0
    assert data["daily"][0]["level"] == "trung bình"
    assert data["daily"][1]["aqi"] == 5
    assert data["daily"][1]["pm2_5"] == 60.0
    assert data["daily"][1]["level"] == "rất xấu"


@pytest.mark.asyncio
async def test_get_air_quality_caps_days_at_four(ftools, monkeypatch, mock_http):
    """AQI chỉ phủ 4 ngày theo giờ -> days=30 vẫn trả về tối đa 4."""
    monkeypatch.setattr(settings, "OPENWEATHER_API_KEY", "test-key")
    _mock_geo(mock_http)
    points = []
    for d in range(6):
        points += _aqi_day(_AQI_DAY1 + d * 86400, 2, 10.0, 20.0)
    mock_http.get(
        url__startswith="https://api.openweathermap.org/data/2.5/air_pollution/forecast"
    ).respond(200, json={"coord": {}, "list": points})
    out = await ftools["get_air_quality"].ainvoke({"location": "Ha Noi", "days": 30})
    data = json.loads(out)
    assert len(data["daily"]) == 4


@pytest.mark.asyncio
async def test_get_air_quality_without_api_key(ftools, monkeypatch):
    monkeypatch.setattr(settings, "OPENWEATHER_API_KEY", None)
    out = await ftools["get_air_quality"].ainvoke({"location": "Ha Noi"})
    assert "OPENWEATHER_API_KEY" in out


def test_aqi_level_uses_openweather_one_to_five_scale():
    assert ext.aqi_level_vi(1) == "tốt"
    assert ext.aqi_level_vi(3) == "trung bình"
    assert ext.aqi_level_vi(5) == "rất xấu"
    assert ext.aqi_level_vi(None) is None


# ------------------------------------------------------------------ trends

def test_trend_slope_labels():
    assert _slope([0, 1, 2, 3, 4, 5, 6]) == "rising"
    assert _slope([6, 5, 4, 3, 2, 1, 0]) == "falling"
    assert _slope([5, 5, 5, 5, 5, 5, 5]) == "flat"
    assert _slope([]) == "flat"
    assert _slope([3]) == "flat"
    # Nhiễu quanh một đường ngang không được báo là tăng/giảm
    assert _slope([50, 45, 55, 48, 52, 47, 51]) == "flat"


@pytest.mark.asyncio
async def test_get_search_trends_happy_path(ftools, monkeypatch):
    monkeypatch.setattr(settings, "TRENDS_ENABLED", True)

    async def _fake_fetch(keywords, location, days):
        return {kw: {"avg": 1.0, "slope": "flat", "timeline": [1, 2]} for kw in keywords}

    monkeypatch.setattr("app.tools.forecast_tools.get_search_trends", _fake_fetch)

    out = await ftools["get_search_trends_tool"].ainvoke(
        {"keywords": "cach tri cum A, thuoc ha sot", "location": "Ha Noi"}
    )
    data = json.loads(out)
    assert data["location"] == "Ha Noi"
    assert {k["keyword"] for k in data["keywords"]} == {"cach tri cum A", "thuoc ha sot"}


@pytest.mark.asyncio
async def test_get_search_trends_graceful_when_unavailable(ftools, monkeypatch):
    async def _none(keywords, location, days):
        return None

    monkeypatch.setattr("app.tools.forecast_tools.get_search_trends", _none)
    out = await ftools["get_search_trends_tool"].ainvoke({"location": "Ha Noi"})
    assert "Khong lay duoc xu huong Google Trends" in out


@pytest.mark.asyncio
async def test_trends_service_swallows_errors(monkeypatch):
    from app.services import trends_service

    monkeypatch.setattr(settings, "TRENDS_ENABLED", True)

    def _boom(*args, **kwargs):
        raise RuntimeError("429 Too Many Requests")

    monkeypatch.setattr(trends_service, "_fetch_sync", _boom)
    assert await trends_service.get_search_trends(["cum A"], "Ha Noi") is None


@pytest.mark.asyncio
async def test_trends_service_respects_disabled_flag(monkeypatch):
    from app.services import trends_service

    monkeypatch.setattr(settings, "TRENDS_ENABLED", False)
    assert await trends_service.get_search_trends(["cum A"], "Ha Noi") is None


# ---------------------------------------------------------------- holidays

@pytest.mark.asyncio
async def test_get_holidays_happy_path(ftools, mock_http):
    import datetime

    today = datetime.date.today()
    soon = (today + datetime.timedelta(days=5)).isoformat()
    later = (today + datetime.timedelta(days=200)).isoformat()

    mock_http.get(url__startswith="https://date.nager.at/api/v3/PublicHolidays").respond(
        200,
        json=[
            {"date": soon, "name": "Test Holiday", "localName": "Ngay test"},
            {"date": later, "name": "Too Far", "localName": "Xa qua"},
        ],
    )
    out = await ftools["get_holidays"].ainvoke({"country_code": "VN", "days_ahead": 30})
    data = json.loads(out)
    assert data["countryCode"] == "VN"
    assert len(data["holidays"]) == 1
    assert data["holidays"][0]["name"] == "Test Holiday"
    assert data["holidays"][0]["daysUntil"] == 5


@pytest.mark.asyncio
async def test_get_holidays_empty(ftools, mock_http):
    mock_http.get(url__startswith="https://date.nager.at/api/v3/PublicHolidays").respond(
        200, json=[]
    )
    out = await ftools["get_holidays"].ainvoke({"days_ahead": 30})
    assert "Khong co le lon nao" in out


@pytest.mark.asyncio
async def test_get_holidays_handles_api_error(ftools, mock_http):
    mock_http.get(url__startswith="https://date.nager.at/api/v3/PublicHolidays").respond(500)
    out = await ftools["get_holidays"].ainvoke({})
    assert "Khong lay duoc du lieu" in out
