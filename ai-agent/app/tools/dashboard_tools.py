import json
from langchain_core.tools import tool

from app.tools.base import api_get
from app.models.chat import AuthContext


def create_dashboard_tools(auth: AuthContext):
    @tool
    async def get_total_revenue() -> str:
        """Lay tong doanh thu cua chi nhanh hien tai."""
        data = await api_get(
            "/api/Dashboard/total-revenue",
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        return json.dumps(data, ensure_ascii=False, default=str)

    @tool
    async def get_total_orders() -> str:
        """Lay tong so hoa don cua chi nhanh hien tai."""
        data = await api_get(
            "/api/Dashboard/total-orders",
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        return json.dumps(data, ensure_ascii=False, default=str)

    @tool
    async def get_revenue_trend(days: int = 7) -> str:
        """Xem xu huong doanh thu theo ngay.
        Args:
            days: So ngay muon xem (mac dinh: 7)
        """
        data = await api_get(
            "/api/Dashboard/revenue-trend",
            params={"days": days},
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        return json.dumps(data, ensure_ascii=False, default=str)

    @tool
    async def get_top_medicines(top: int = 10) -> str:
        """Xem top thuoc ban chay nhat.
        Args:
            top: So luong top (mac dinh: 10)
        """
        data = await api_get(
            "/api/Dashboard/top-medicines",
            params={"top": top},
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        items = data.get("Items", []) if isinstance(data, dict) else data
        if not items:
            return "Khong co du lieu top thuoc."
        return json.dumps(items, ensure_ascii=False, default=str)

    @tool
    async def get_revenue_by_branch() -> str:
        """Xem doanh thu theo tung chi nhanh (chi admin moi co)."""
        data = await api_get(
            "/api/Dashboard/revenue-by-branch",
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        return json.dumps(data, ensure_ascii=False, default=str)

    @tool
    async def get_inventory_capital() -> str:
        """Xem gia tri ton kho hien tai."""
        data = await api_get(
            "/api/Dashboard/inventory-capital",
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        return json.dumps(data, ensure_ascii=False, default=str)

    @tool
    async def get_new_customers() -> str:
        """Xem so luong khach hang moi."""
        data = await api_get(
            "/api/Dashboard/new-customers",
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        return json.dumps(data, ensure_ascii=False, default=str)

    @tool
    async def get_employee_progress() -> str:
        """Xem tien do ban hang cua nhan vien trong ngay."""
        data = await api_get(
            "/api/Dashboard/employee-progress",
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        items = data.get("Items", []) if isinstance(data, dict) else data
        if not items:
            return "Khong co du lieu tien do nhan vien."
        return json.dumps(items, ensure_ascii=False, default=str)

    @tool
    async def get_dashboard_summary() -> str:
        """Lay tong hop tat ca KPI cua dashboard: doanh thu, hoa don, khach hang, ton kho, canh bao."""
        results = {}
        for name, path in [
            ("revenue", "/api/Dashboard/total-revenue"),
            ("orders", "/api/Dashboard/total-orders"),
            ("customers", "/api/Dashboard/new-customers"),
            ("capital", "/api/Dashboard/inventory-capital"),
        ]:
            data = await api_get(path, token=auth.token, branch_id=auth.branch_id)
            results[name] = data
        return json.dumps(results, ensure_ascii=False, default=str)

    return [
        get_total_revenue,
        get_total_orders,
        get_revenue_trend,
        get_top_medicines,
        get_revenue_by_branch,
        get_inventory_capital,
        get_new_customers,
        get_employee_progress,
        get_dashboard_summary,
    ]
