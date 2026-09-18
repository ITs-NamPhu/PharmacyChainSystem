import json
from typing import Any

from app.services.backend_client import backend_client, BackendError


def _to_json(data: Any, limit: int = 50) -> str:
    if isinstance(data, list):
        return json.dumps(data[:limit], ensure_ascii=False, default=str)
    return json.dumps(data, ensure_ascii=False, default=str)


async def api_get(path: str, params: dict = None,
                  token: str = None, branch_id: str = None) -> Any:
    try:
        return await backend_client.get(path, params=params,
                                        token=token, branch_id=branch_id)
    except BackendError as e:
        return {"error": str(e)}


async def api_post(path: str, json_body: dict = None,
                   token: str = None, branch_id: str = None) -> Any:
    try:
        return await backend_client.post(path, json_body=json_body,
                                         token=token, branch_id=branch_id)
    except BackendError as e:
        return {"error": str(e)}
