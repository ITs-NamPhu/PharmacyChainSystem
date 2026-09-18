import json
from langchain_core.tools import tool

from app.tools.base import api_get
from app.models.chat import AuthContext


def create_customer_tools(auth: AuthContext):
    @tool
    async def search_customer(query: str = "") -> str:
        """Tim kiem khach hang theo ten hoac so dien thoai.
        Args:
            query: Ten hoac so dien thoai khach hang (de trong = xem tat ca)
        """
        data = await api_get(
            "/api/Customer/All",
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        if query:
            data = [
                c for c in data
                if query.lower() in (c.get("CustomerName", "")).lower()
                or query in (c.get("Phone", "") or "")
            ]
        if not data:
            return "Khong tim thay khach hang."
        return json.dumps(data[:20], ensure_ascii=False, default=str)

    @tool
    async def get_customer_detail(customer_id: int) -> str:
        """Xem chi tiet khach hang.
        Args:
            customer_id: ID cua khach hang
        """
        data = await api_get(
            f"/api/Customer/{customer_id}",
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        return json.dumps(data, ensure_ascii=False, default=str)

    return [search_customer, get_customer_detail]
