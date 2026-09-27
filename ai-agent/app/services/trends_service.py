"""Bọc Google Trends (pytrends) cho agent dự báo nhập hàng.

Trend tìm kiếm thường đi trước nhu cầu mua thuốc 2-3 ngày nên là tín hiệu
dịch bệnh sớm cho một tỉnh/thành cụ thể.

Lưu ý: pytrends là thư viện không chính thức, blocking và dễ bị giới hạn
tần suất (429). Vì vậy:
- chạy trong thread pool để không chặn event loop,
- mọi lỗi đều bị nuốt và trả về None để tool báo "không lấy được",
- có thể tắt hoàn toàn bằng settings.TRENDS_ENABLED.
"""

import asyncio
import logging
import re
from typing import Optional

from app.config.settings import settings

logger = logging.getLogger(__name__)

# "today 3-m" | "today 7-d" | "today 1-m" | "today 3-m" | "today 12-m" | "today 5-y"
_RANGE_BY_DAYS = {
    1: "today 1-d",
    2: "today 7-d",
    3: "today 7-d",
    7: "today 7-d",
    14: "today 1-m",
    30: "today 1-m",
    90: "today 3-m",
    365: "today 12-m",
}


def _resolve_range(days: int) -> str:
    return _RANGE_BY_DAYS.get(days, "today 1-m")


def _build_geo(location: str) -> str:
    """pytrends cần định mã địa lý dạng 'Tỉnh,VN' hoặc mã số vùng (geo=VN-44)."""
    return f"{location.strip()},VN"


def _slope(values: list[int]) -> str:
    """Hồi quy tuyến tính rút gọn trên chuỗi interest, chỉ trả nhãn."""
    n = len(values)
    if n < 2:
        return "flat"

    sum_x = sum_y = sum_xy = sum_x2 = 0.0
    for i, y in enumerate(values):
        x = i + 1
        sum_x += x
        sum_y += y
        sum_xy += x * y
        sum_x2 += x * x

    denominator = (n * sum_x2) - (sum_x * sum_x)
    if denominator == 0:
        return "flat"

    slope = ((n * sum_xy) - (sum_x * sum_y)) / denominator
    # Interest 0-100: đi lệch 0.5 điểm/ngày đã là có ý nghĩa
    threshold = max(0.5, abs(sum_y / n) * 0.05)

    if slope > threshold:
        return "rising"
    if slope < -threshold:
        return "falling"
    return "flat"


def _parse_interest(blob: str) -> list[int]:
    """pytrends trả về chuỗi JSON dạng '[[date, value], ...]'."""
    import json

    try:
        rows = json.loads(blob)
    except (ValueError, TypeError):
        return []
    return [int(v) for _, v in rows if isinstance(v, (int, float))]


def _fetch_sync(keywords: list[str], location: str, days: int) -> dict:
    """Phần blocking, chạy trong thread. Trả về dict hoặc None nếu lỗi."""
    from pytrends.request import TrendReq

    pytrends = TrendReq(hl="vi-VN", tz=0, timeout=(10, 25))
    pytrends.build_payload(
        kw_list=keywords,
        timeframe=_resolve_range(days),
        geo=_build_geo(location),
    )
    return pytrends.interest_over_time()


def _find_column(frame, keyword: str) -> Optional[str]:
    for column in frame.columns:
        if str(column).strip().lower() == keyword.strip().lower():
            return column
    # Cho phép khớp gần đúng (khác dấu / bỏ khoảng trắng thừa)
    target = re.sub(r"\s+", " ", keyword.strip().lower())
    for column in frame.columns:
        if re.sub(r"\s+", " ", str(column).strip().lower()) == target:
            return column
    return None


async def get_search_trends(
    keywords: list[str], location: str, days: int = 7
) -> Optional[dict]:
    """Lấy interest-over-time cho nhiều từ khóa tại một tỉnh/thành.

    Trả về ``{keyword: {"avg": float, "slope": str, "timeline": [int]}}``
    hoặc None nếu không lấy được.
    """
    if not settings.trends_available:
        logger.info("Google Trends bị tắt qua settings.TRENDS_ENABLED")
        return None

    cleaned = [k.strip() for k in keywords if k and k.strip()][:5]
    if not cleaned:
        return None

    try:
        frame = await asyncio.to_thread(_fetch_sync, cleaned, location, days)
    except Exception as e:  # noqa: BLE001 - pytrends lỗi rất đa dạng, không được làm chết agent
        logger.warning(f"Google Trends that bai: {e}")
        return None

    if frame is None or getattr(frame, "empty", True):
        return None

    result: dict[str, dict] = {}
    for keyword in cleaned:
        column = _find_column(frame, keyword)
        if column is None:
            continue
        values = [int(v) for v in frame[column].tolist()]
        if not values:
            continue
        result[keyword] = {
            "avg": round(sum(values) / len(values), 2),
            "slope": _slope(values),
            "timeline": values,
        }

    return result or None
