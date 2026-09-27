"""Test gọi API THẬT cho phần dự báo.

Mặc định bị skip vì phụ thuộc mạng, quota OpenWeather và việc Google Trends
chặn request tự động. Chạy khi có:

    $env:RUN_LIVE_TESTS='1'; python -m pytest tests/test_forecast_live.py -q -rs

Test ở đây chỉ kiểm tra *hình dạng dữ liệu* khớp với những gì parser đang giả
định. Nếu OpenWeather đổi schema, đây chính là chỗ báo trước cho ta.
"""

import json
import os
from datetime import datetime, timezone

import pytest

from app.services import external_api_client as ext
from app.tools.forecast_tools import _FORECAST_POINTS_PER_DAY, create_forecast_tools

pytestmark = [
    pytest.mark.live,
    pytest.mark.skipif(
        os.getenv("RUN_LIVE_TESTS") != "1",
        reason="goi API that; bat bang RUN_LIVE_TESTS=1",
    ),
]

_LOCATION = "Ha Noi"
_VN_TZ = 25200


@pytest.fixture(autouse=True)
async def _fresh_external_state():
    """Mỗi test là một event loop riêng, còn httpx client được cache ở module
    scope nên phải reset để không dùng lại connection của loop đã đóng."""
    ext.clear_cache()
    yield
    await ext.reset_client()
    ext.clear_cache()


@pytest.fixture
def ftools(auth):
    return {t.name: t for t in create_forecast_tools(auth)}


# --------------------------------------------------------------- geocoding

@pytest.mark.asyncio
async def test_live_geocode_hanoi():
    place = await ext.geocode(_LOCATION)
    assert place is not None, f"geocode '{_LOCATION}' tra ve None"
    assert 20.0 < place["lat"] < 22.5, place
    assert 105.0 < place["lon"] < 106.5, place


# ------------------------------------------------------------------ thoi tiet

@pytest.mark.asyncio
async def test_live_weather_endpoint_shape():
    """Xác nhận các giả định của ``_group_forecast_by_day`` vẫn đúng với API thật."""
    payload = await ext.get_weather_forecast(_LOCATION)
    assert payload, "tra ve rong"

    # chuỗi 3 gio/lan
    assert payload["city"]["timezone"] == _VN_TZ, payload["city"]
    points = payload["list"]
    assert len(points) == 40, f"ky vong 40 moc, nhan {len(points)}"
    steps = {points[i + 1]["dt"] - points[i]["dt"] for i in range(len(points) - 1)}
    assert steps == {10800}, f"buoc moc khong phai 3 gio: {steps}"

    # nhiet do/do am nam trong `main`, khong phai top-level
    first = points[0]
    assert "temp" in first["main"] and "humidity" in first["main"]
    # dieu kien thoi tiet nam trong `weather[0]`
    assert first["weather"][0]["main"]
    # mua la {"3h": mm}
    raining = [p for p in points if "rain" in p]
    assert raining, "khong co moc nao co mua de kiem tra rain['3h']"
    assert all("3h" in p["rain"] for p in raining), raining[0]["rain"]


@pytest.mark.asyncio
async def test_live_weather_groups_into_full_days(ftools):
    """Với chuỗi thật, tool phải ra đúng các ngày đầy đủ 8 moc."""
    out = await ftools["get_weather_forecast"].ainvoke({"location": _LOCATION, "days": 4})
    data = json.loads(out)
    daily = data["daily"]
    assert 1 <= len(daily) <= 4, daily
    for day in daily:
        assert day["tmin"] is not None and day["tmax"] is not None, day
        assert day["tmin"] <= day["tmax"], day
        assert 0 <= day["humidity"] <= 100, day
        assert day["rainMm"] >= 0, day


@pytest.mark.asyncio
async def test_live_weather_day_count_matches_full_days_in_payload(ftools):
    """Số ngày tool trả về phải khớp số ngày đủ 8 moc trong payload thật.

    Đây là kiểm tra quan trọng nhất: nó bắt được lỗi "cắt ``[:limit]`` trước khi
    loại ngày lương" khiến mất một ngày dù API vẫn đủ 4 ngày.
    """
    payload = await ext.get_weather_forecast(_LOCATION)
    assert payload
    tz = payload["city"]["timezone"]

    counts: dict[str, int] = {}
    for entry in payload["list"]:
        key = datetime.fromtimestamp(
            int(entry["dt"]) + tz, tz=timezone.utc
        ).date().isoformat()
        counts[key] = counts.get(key, 0) + 1

    full_days = [d for d, c in counts.items() if c >= _FORECAST_POINTS_PER_DAY]
    assert len(full_days) == 4, counts  # 4 ngay day du trong chuoi 40 moc that

    out = await ftools["get_weather_forecast"].ainvoke({"location": _LOCATION, "days": 4})
    returned = json.loads(out)["daily"]
    assert [d["date"] for d in returned] == full_days[:4], (returned, full_days)


# ------------------------------------------------------------------------ AQI

@pytest.mark.asyncio
async def test_live_air_pollution_endpoint_shape():
    """``main`` chỉ có aqi 1-5; PM nằm trong ``components``; bước 1 giờ."""
    payload = await ext.get_air_pollution_forecast(_LOCATION)
    assert payload, "tra ve rong"
    points = payload["list"]
    assert len(points) >= 90, f"AQI it nhat ~4 ngay theo gio, nhan {len(points)}"
    steps = {points[i + 1]["dt"] - points[i]["dt"] for i in range(len(points) - 1)}
    assert steps == {3600}, f"buoc moc khong phai 1 gio: {steps}"
    for entry in points[:5]:
        assert set(entry["main"]) == {"aqi"}, entry["main"]
        assert 1 <= entry["main"]["aqi"] <= 5, entry["main"]
        assert entry["components"]["pm2_5"] is not None


@pytest.mark.asyncio
async def test_live_air_quality_returns_four_days(ftools):
    out = await ftools["get_air_quality"].ainvoke({"location": _LOCATION, "days": 4})
    data = json.loads(out)
    assert len(data["daily"]) == 4, data["daily"]
    for day in data["daily"]:
        assert 1 <= day["aqi"] <= 5, day
        assert day["pm2_5"] is not None, day
        assert 0 <= day["pm2_5"] <= 1000, day
