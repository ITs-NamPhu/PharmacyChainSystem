import json
from langchain_core.tools import tool

from app.tools.base import api_get
from app.models.chat import AuthContext


def create_warehouse_tools(auth: AuthContext):
    @tool
    async def get_stock_batches(medicine_name: str = "") -> str:
        """Xem ton kho cac lo thuoc trong kho chi nhanh hien tai.
        Args:
            medicine_name: Ten thuoc cu the (de trong = xem tat ca)
        """
        data = await api_get(
            "/api/Warehouse/batches",
            params={"page": 1, "count": 100},
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        batches = data.get("Batches", []) if isinstance(data, dict) else data
        if medicine_name:
            batches = [
                b for b in batches
                if medicine_name.lower() in (b.get("MedicineName", "")).lower()
            ]
        if not batches:
            return "Khong co lo thuoc nao trong kho."
        return json.dumps(batches[:30], ensure_ascii=False, default=str)

    @tool
    async def get_batches_by_medicine(medicine_id: int) -> str:
        """Xem cac lo thuoc cua mot thuoc cu the trong chi nhanh.
        Args:
            medicine_id: ID cua thuoc
        """
        data = await api_get(
            "/api/Invoice/BatchesByMedicine",
            params={"medicineID": medicine_id},
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        if not data:
            return "Khong co lo nao cho thuoc nay."
        return json.dumps(data, ensure_ascii=False, default=str)

    @tool
    async def get_fefo_allocation(medicine_id: int, quantity: int) -> str:
        """Phan bo lo thuoc theo phuong phap FEFO (First-Expired-First-Out).
        Args:
            medicine_id: ID cua thuoc
            quantity: So luong can phan bo
        """
        data = await api_get(
            "/api/Invoice/FefoBatches",
            params={"medicineID": medicine_id, "quantity": quantity},
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        return json.dumps(data, ensure_ascii=False, default=str)

    @tool
    async def get_expiring_batches() -> str:
        """Xem cac lo thuoc sap het han (3-6 thang toi)."""
        data = await api_get(
            "/api/Dashboard/expiring-batches",
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        items = data.get("Items", []) if isinstance(data, dict) else data
        if not items:
            return "Khong co lo thuoc nao sap het han."
        return json.dumps(items, ensure_ascii=False, default=str)

    @tool
    async def get_low_stock(threshold: int = 10) -> str:
        """Canh bao thuoc ton kho thap, duoi nguong qui dinh.
        Args:
            threshold: Nguong canh bao (mac dinh: 10)
        """
        data = await api_get(
            "/api/Dashboard/low-stock",
            params={"threshold": threshold},
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        items = data.get("Items", []) if isinstance(data, dict) else data
        if not items:
            return f"Khong co thuoc nao ton kho duoi {threshold}."
        return json.dumps(items, ensure_ascii=False, default=str)

    @tool
    async def get_destroy_queue() -> str:
        """Xem danh sach thuoc da het han, cho tieu huy."""
        data = await api_get(
            "/api/Dashboard/destroy-queue",
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        items = data.get("Items", []) if isinstance(data, dict) else data
        if not items:
            return "Khong co thuoc nao cho tieu huy."
        return json.dumps(items, ensure_ascii=False, default=str)

    @tool
    async def get_out_of_stock() -> str:
        """Xem danh sach thuoc het hang (ton kho = 0)."""
        data = await api_get(
            "/api/Dashboard/out-of-stock",
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        items = data.get("Items", []) if isinstance(data, dict) else data
        if not items:
            return "Khong co thuoc nao het hang."
        return json.dumps(items, ensure_ascii=False, default=str)

    return [
        get_stock_batches,
        get_batches_by_medicine,
        get_fefo_allocation,
        get_expiring_batches,
        get_low_stock,
        get_destroy_queue,
        get_out_of_stock,
    ]
