import json
from langchain_core.tools import tool

from app.tools.base import api_get
from app.models.chat import AuthContext


def create_invoice_tools(auth: AuthContext):
    @tool
    async def search_invoice(page: int = 1, count: int = 10) -> str:
        """Xem danh sach hoa don ban hang (phan trang).
        Args:
            page: So trang (mac dinh: 1)
            count: So luong moi trang (mac dinh: 10)
        """
        data = await api_get(
            "/api/Invoice/GetAll",
            params={"page": page, "count": count},
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        return json.dumps(data, ensure_ascii=False, default=str)

    @tool
    async def get_invoice_detail(invoice_id: int) -> str:
        """Xem chi tiet mot hoa don.
        Args:
            invoice_id: ID cua hoa don
        """
        data = await api_get(
            f"/api/Invoice/{invoice_id}",
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        return json.dumps(data, ensure_ascii=False, default=str)

    return [search_invoice, get_invoice_detail]
