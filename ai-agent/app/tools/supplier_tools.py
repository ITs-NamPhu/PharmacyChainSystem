import json
from langchain_core.tools import tool

from app.tools.base import api_get, get_field
from app.models.chat import AuthContext


def create_supplier_tools(auth: AuthContext):
    @tool
    async def search_supplier(query: str = "") -> str:
        """NHÀ CUNG CẤP - Tìm kiếm nhà cung cấp theo tên.
        Kết quả gồm: SupplierID, SupplierName, Phone, Email, Address.
        Args:
            query: Tên nhà cung cấp (để trống = xem tất cả)

        KẾT HỢP ENTITY:
        - "nhà cung cấp X" → query="X".
        - "phiếu nhập hàng của nhà cung cấp X" → dùng search_goods_receipts(keyword="X").
        """
        data = await api_get(
            "/api/Supplier/GetAll",
            params={"page": 1, "count": 50},
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        suppliers = get_field(data, "Suppliers", []) if isinstance(data, dict) else data
        if query:
            suppliers = [
                s for s in suppliers
                if query.lower() in (s.get("SupplierName", "")).lower()
            ]
        if not suppliers:
            return "Khong tim thay nha cung cap."
        return json.dumps(suppliers[:20], ensure_ascii=False, default=str)

    return [search_supplier]
