"""HTTP client cho các API ngoài phục vụ agent dự báo nhập hàng.

Gồm OpenWeather (geocoding, thời tiết, chất lượng không khí) và Nager.Date
(lịch nghỉ lễ quốc gia). Tất cả lời gọi đều có cache TTL trong bộ nhớ để tránh
đốt quota và tránh gọi lặp khi agent vòng lặp nhiều lượt.

Về thời tiết: dùng ``/data/2.5/forecast`` (gói miễn phí) thay vì One Call 3.0,
vì One Call 3.0 bắt buộc mua gói "One Call by Call" riêng và trả 401 nếu tài
khoản chưa đăng ký. Endpoint này trả dữ liệu 3 giờ/lần nên tool phải tự gom về
từng ngày.
"""

import asyncio
import logging
import time
from typing import Any, Optional

import httpx

from app.config.settings import settings

logger = logging.getLogger(__name__)

_cache: dict[str, tuple[float, Any]] = {}


def _cache_get(key: str) -> Optional[Any]:
    entry = _cache.get(key)
    if entry is None:
        return None
    expires_at, value = entry
    if expires_at < time.time():
        _cache.pop(key, None)
        return None
    return value


def _cache_set(key: str, value: Any, ttl: int) -> Any:
    _cache[key] = (time.time() + ttl, value)
    return value


def clear_cache() -> None:
    _cache.clear()


async def close_client() -> None:
    await _get_client().aclose()


_client: Optional[httpx.AsyncClient] = None
_client_lock = asyncio.Lock()


async def _get_client() -> httpx.AsyncClient:
    global _client
    if _client is not None and not _client.is_closed:
        return _client
    async with _client_lock:
        if _client is None or _client.is_closed:
            _client = httpx.AsyncClient(timeout=settings.EXTERNAL_API_TIMEOUT)
    return _client


async def reset_client() -> None:
    """Đóng và bỏ client dùng chung.

    Client được cache ở module scope nên gắn với event loop đã tạo ra nó. Khi
    chạy test, mỗi test là một loop riêng, nên client của test trước sẽ treo vào
    loop đã đóng và mọi request sau đó ném ``Event loop is closed``. Hàm này
    cho phép cô lập trạng thái giữa các test.
    """
    global _client
    if _client is not None and not _client.is_closed:
        await _client.aclose()
    _client = None


async def _get_json(url: str, params: dict, cache_key: str, ttl: int) -> Any:
    cached = _cache_get(cache_key)
    if cached is not None:
        return cached

    client = await _get_client()
    resp = await client.get(url, params=params)
    resp.raise_for_status()
    data = resp.json()
    return _cache_set(cache_key, data, ttl)


# ---------------------------------------------------------------- geocoding

async def geocode(location: str) -> Optional[dict]:
    """Đổi tên tỉnh/thành ('Ha Noi') sang toạ độ. Trả None nếu không tìm thấy."""
    if not settings.openweather_enabled:
        return None

    query = (location or "").strip()
    if not query:
        return None

    cache_key = f"geo:{query.lower()}"
    data = await _get_json(
        "https://api.openweathermap.org/geo/1.0/direct",
        {"q": query, "limit": 1, "appid": settings.OPENWEATHER_API_KEY},
        cache_key,
        settings.GEOCODE_CACHE_TTL,
    )

    if not data:
        return None
    return data[0]


# ---------------------------------------------------------------- thời tiết

_CONDITION_VI = {
    "clear": "trời quang",
    "clouds": "nhiều mây",
    "rain": "mưa",
    "drizzle": "mưa phùn",
    "thunderstorm": "giông",
    "snow": "tuyết rơi",
    "mist": "sương mù",
    "fog": "sương mù",
    "haze": "sương mờ",
    "dust": "bụi",
    "sand": "cát bụi",
    "ash": "tro bụi",
    "squall": "gió giật",
    "tornado": "lốc xoáy",
}


def condition_vi(main: Optional[str], description: Optional[str]) -> str:
    if main and main.lower() in _CONDITION_VI:
        return _CONDITION_VI[main.lower()]
    if description:
        return description
    return main or "không rõ"


async def get_weather_forecast(location: str, days: int = 4) -> Optional[dict]:
    """Dự báo thời tiết qua OpenWeather 5-day forecast (gói miễn phí).

    Trả về nguyên payload của API. Lưu ý: dữ liệu theo bước 3 giờ, và ngày
    đầu tiên thường không đầy đủ vì chuỗi dự báo bắt đầu từ thời điểm hiện tại.
    """
    if not settings.openweather_enabled:
        return None

    place = await geocode(location)
    if not place:
        return None

    lat, lon = place.get("lat"), place.get("lon")
    cache_key = f"fc25:{lat},{lon}"
    cached = _cache_get(cache_key)
    if cached is not None:
        return cached

    client = await _get_client()
    resp = await client.get(
        "https://api.openweathermap.org/data/2.5/forecast",
        params={
            "lat": lat,
            "lon": lon,
            "units": "metric",
            "lang": "vi",
            "appid": settings.OPENWEATHER_API_KEY,
        },
    )
    resp.raise_for_status()
    return _cache_set(cache_key, resp.json(), settings.EXTERNAL_CACHE_TTL)


# ------------------------------------------------------- chất lượng không khí

# OpenWeather trả main.aqi theo thang 1-5, KHÔNG phải AQI 1-500 của Mỹ
_AQI_LEVEL_VI = {
    1: "tốt",
    2: "ổn định",
    3: "trung bình",
    4: "xấu",
    5: "rất xấu",
}


def aqi_level_vi(aqi: Optional[int]) -> Optional[str]:
    if aqi is None:
        return None
    return _AQI_LEVEL_VI.get(int(aqi), "không rõ")


async def get_air_pollution_forecast(location: str, days: int = 4) -> Optional[dict]:
    """Dự báo chất lượng không khí (AQI, PM2.5, PM10) qua OpenWeather Air Pollution.

    Cấu trúc mỗi phần tử trong ``list`` (đã đối chiếu response thật):
    ``{"dt": <unix>, "main": {"aqi": 1-5}, "components": {"pm2_5": .., "pm10": .., ...}}``
    ⇒ ``aqi`` nằm trong ``main``, còn nồng độ bụi nằm trong ``components``.
    Dữ liệu theo giờ, phủ 4 ngày.
    """
    if not settings.openweather_enabled:
        return None

    place = await geocode(location)
    if not place:
        return None

    lat, lon = place.get("lat"), place.get("lon")
    cache_key = f"air:{lat},{lon}:{days}"
    cached = _cache_get(cache_key)
    if cached is not None:
        return cached

    client = await _get_client()
    resp = await client.get(
        "https://api.openweathermap.org/data/2.5/air_pollution/forecast",
        params={"lat": lat, "lon": lon, "appid": settings.OPENWEATHER_API_KEY},
    )
    resp.raise_for_status()
    return _cache_set(cache_key, resp.json(), settings.EXTERNAL_CACHE_TTL)


# ------------------------------------------------------------ lịch nghỉ lễ

async def get_public_holidays(country_code: str, year: int) -> list:
    """Lịch nghỉ lễ quốc gia từ Nager.Date (không cần API key)."""
    code = (country_code or "VN").upper()
    cache_key = f"holidays:{code}:{year}"
    cached = _cache_get(cache_key)
    if cached is not None:
        return cached

    client = await _get_client()
    resp = await client.get(
        f"https://date.nager.at/api/v3/PublicHolidays/{year}/{code}"
    )
    resp.raise_for_status()
    data = resp.json()
    if not isinstance(data, list):
        return _cache_set(cache_key, [], settings.EXTERNAL_CACHE_TTL)
    return _cache_set(cache_key, data, settings.EXTERNAL_CACHE_TTL)
