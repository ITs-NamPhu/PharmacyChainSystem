import json
from langchain_core.tools import tool

from app.tools.base import api_get
from app.models.chat import AuthContext


def create_medicine_tools(auth: AuthContext):
    @tool
    async def search_medicine(query: str) -> str:
        """Tim kiem thuoc theo ten hoac ma thuoc trong he thong nha thuoc.
        Args:
            query: Ten hoac ma thuoc can tim
        """
        data = await api_get(
            "/api/Medicine/All",
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        results = [
            m for m in data
            if query.lower() in (m.get("MedicineName", "")).lower()
        ]
        if not results:
            return f"Khong tim thay thuoc voi tu khoa '{query}'"
        return json.dumps(results[:10], ensure_ascii=False, default=str)

    @tool
    async def get_medicine_detail(medicine_id: int) -> str:
        """Xem chi tiet mot loai thuoc theo ID.
        Args:
            medicine_id: ID cua thuoc
        """
        data = await api_get(
            f"/api/Medicine/{medicine_id}",
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        return json.dumps(data, ensure_ascii=False, default=str)

    return [search_medicine, get_medicine_detail]
