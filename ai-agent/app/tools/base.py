import json
from typing import Any

from app.services.backend_client import backend_client, BackendError


def _to_json(data: Any, limit: int = 50) -> str:
    if isinstance(data, list):
        return json.dumps(data[:limit], ensure_ascii=False, default=str)
    return json.dumps(data, ensure_ascii=False, default=str)


def clean_payload(payload: dict) -> dict:
    """Loại bỏ các key có giá trị None để request gọn gàng."""
    return {k: v for k, v in payload.items() if v is not None}


def get_field(data: Any, name: str, default: Any = None) -> Any:
    """Lấy field không phân biệt hoa/thường.

    Backend .NET trả JSON camelCase (VD: 'items', 'customerId') trong khi code
    tool đọc theo PascalCase ('Items', 'CustomerId'). Hàm này xử lý cả hai.
    """
    if not isinstance(data, dict):
        return default
    if name in data:
        return data[name]
    lowered = name.lower()
    for key, value in data.items():
        if isinstance(key, str) and key.lower() == lowered:
            return value
    return default


async def api_get(path: str, params: dict = None,
                  token: str = None, branch_id: str = None) -> Any:
    try:
        return await backend_client.get(path, params=params,
                                        token=token, branch_id=branch_id)
    except BackendError as e:
        if e.ec == -999:  # access token hết hạn -> bật cờ cho chat.py gửi auth_error
            backend_client.token_expired = True
        return {"error": str(e)}


async def api_post(path: str, json_body: dict = None,
                   token: str = None, branch_id: str = None) -> Any:
    try:
        return await backend_client.post(path, json_body=json_body,
                                         token=token, branch_id=branch_id)
    except BackendError as e:
        if e.ec == -999:
            backend_client.token_expired = True
        return {"error": str(e)}
