import json
from langchain_core.tools import tool

from app.tools.base import api_get
from app.models.chat import AuthContext


def create_branch_tools(auth: AuthContext):
    @tool
    async def list_branches() -> str:
        """Xem danh sach tat ca cac chi nhanh nha thuoc."""
        data = await api_get(
            "/api/Branch/All",
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        if not data:
            return "Khong co chi nhanh nao."
        return json.dumps(data, ensure_ascii=False, default=str)

    return [list_branches]
