import json
from langchain_core.tools import tool

from app.tools.base import api_get
from app.models.chat import AuthContext


def create_supplier_tools(auth: AuthContext):
    @tool
    async def search_supplier(query: str = "") -> str:
        """Tim kiem nha cung cap theo ten.
        Args:
            query: Ten nha cung cap (de trong = xem tat ca)
        """
        data = await api_get(
            "/api/Supplier/GetAll",
            params={"page": 1, "count": 50},
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        suppliers = data.get("Suppliers", []) if isinstance(data, dict) else data
        if query:
            suppliers = [
                s for s in suppliers
                if query.lower() in (s.get("SupplierName", "")).lower()
            ]
        if not suppliers:
            return "Khong tim thay nha cung cap."
        return json.dumps(suppliers[:20], ensure_ascii=False, default=str)

    return [search_supplier]
