import json
import logging
import time
from typing import Any, Optional

import httpx

from app.config.settings import settings

logger = logging.getLogger(__name__)


class BackendError(Exception):
    def __init__(self, message: str, status_code: int = 500, ec=None):
        self.message = message
        self.status_code = status_code
        self.ec = ec
        super().__init__(self.message)


class BackendClient:
    def __init__(self, base_url: str = None):
        self.base_url = base_url or settings.BACKEND_URL
        self.client = httpx.AsyncClient(base_url=self.base_url, timeout=30.0)
        self.token_expired = False

    async def close(self):
        await self.client.aclose()

    async def request(
        self,
        method: str,
        path: str,
        params: dict = None,
        json_body: Any = None,
        token: str = None,
        branch_id: str = None,
    ) -> Any:
        t0 = time.perf_counter()

        def log_result(level: str, message: str):
            elapsed_ms = (time.perf_counter() - t0) * 1000
            getattr(logger, level)(f"{method} {path} - {elapsed_ms:.1f}ms - {message}")

        headers = {}
        if token:
            # .NET yêu cầu "Bearer " prefix để xác thực JWT
            headers["Authorization"] = token if token.lower().startswith("bearer ") else f"Bearer {token}"
        if branch_id:
            headers["X-Branch-Id"] = branch_id

        try:
            resp = await self.client.request(
                method,
                path,
                params=params,
                json=json_body,
                headers=headers,
            )
        except httpx.RequestError as e:
            log_result("error", f"request failed - {e}")
            raise BackendError(f"Không thể kết nối đến backend: {e}")

        try:
            data = resp.json()
        except ValueError:
            snippet = (resp.text or "")[:300]
            log_result("error", f"non-JSON - HTTP {resp.status_code} - {snippet}")
            raise BackendError(
                message=f"Backend tra loi HTTP {resp.status_code}: {snippet}",
                status_code=resp.status_code,
            )

        if not isinstance(data, dict):
            log_result("debug", f"HTTP {resp.status_code} non-dict payload")
            return data

        ec = data.get("EC") or data.get("ec")
        if ec is not None and ec != 0:
            # EC=-999 = access token hết hạn -> bật cờ báo cho chat.py gửi auth_error
            if ec == -999:
                self.token_expired = True
            em = data.get("EM") or data.get("em", "Unknown error")
            log_result("warning", f"EC={ec} EM={em}")
            raise BackendError(message=em, status_code=resp.status_code, ec=ec)

        if resp.status_code >= 400:
            em = data.get("EM") or data.get("em")
            log_result("warning", f"HTTP {resp.status_code} - {em}")
            raise BackendError(
                message=em or f"Backend tra loi HTTP {resp.status_code}",
                status_code=resp.status_code,
            )

        log_result("info", f"HTTP {resp.status_code} ok")
        if "DT" in data:
            return data["DT"]
        if "dt" in data:
            return data["dt"]
        return data

    async def get(self, path: str, params: dict = None,
                  token: str = None, branch_id: str = None) -> Any:
        return await self.request("GET", path, params=params,
                                  token=token, branch_id=branch_id)

    async def post(self, path: str, json_body: Any = None,
                   token: str = None, branch_id: str = None) -> Any:
        return await self.request("POST", path, json_body=json_body,
                                  token=token, branch_id=branch_id)

    async def put(self, path: str, json_body: Any = None,
                  params: dict = None, token: str = None,
                  branch_id: str = None) -> Any:
        return await self.request("PUT", path, params=params,
                                  json_body=json_body,
                                  token=token, branch_id=branch_id)

    async def delete(self, path: str, params: dict = None,
                     token: str = None, branch_id: str = None) -> Any:
        return await self.request("DELETE", path, params=params,
                                  token=token, branch_id=branch_id)


backend_client = BackendClient()
