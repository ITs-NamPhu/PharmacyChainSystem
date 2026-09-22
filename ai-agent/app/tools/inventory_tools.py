import json
from typing import Optional
from langchain_core.tools import tool

from app.tools.base import api_get, api_post, clean_payload, get_field
from app.models.chat import AuthContext


def create_inventory_tools(auth: AuthContext):
    @tool
    async def search_inventory(
        keyword: Optional[str] = None,
        medicine_id: Optional[int] = None,
        category_id: Optional[int] = None,
        manufacturer_id: Optional[int] = None,
        warehouse_id: Optional[int] = None,
        is_expiring_soon: Optional[bool] = None,
        in_stock: Optional[bool] = None,
        out_of_stock: Optional[bool] = None,
        is_low_stock: Optional[bool] = None,
        stock_threshold: Optional[float] = None,
        min_price: Optional[float] = None,
        max_price: Optional[float] = None,
        count: Optional[int] = None,
    ) -> str:
        """
        THUỐC & TỒN KHO - Tra cứu thuốc và tồn kho theo từng LÔ (batch) tại chi nhánh.
        Kết quả trả về từng lô thuốc gồm: MedicineId, MedicineName, UnitName, CategoryName, ManufacturerName, BatchID, QuantityInStock, ExpiryDate, Price, WarehouseName, BranchName.
        Args:
            keyword: Tên thuốc hoặc tên nhà sản xuất cần tìm
            medicine_id: ID cụ thể của thuốc
            category_id: ID nhóm thuốc
            manufacturer_id: ID nhà sản xuất
            warehouse_id: ID kho cụ thể (de trong = tất cả kho của chi nhánh)
            is_expiring_soon: True = chỉ lấy thuốc sắp hết hạn (3-6 tháng tới)
            in_stock: True = chỉ lấy thuốc còn hàng
            out_of_stock: True = chỉ lấy thuốc hết hàng
            is_low_stock: True = chỉ lấy thuốc tồn kho thấp dưới ngưỡng
            stock_threshold: Ngưỡng tồn kho thấp (mặc định 10, chỉ dùng khi is_low_stock=True)
            min_price: Giá bán tối thiểu
            max_price: Giá bán tối đa
            count: Số kết quả tối đa (mặc định 20)

        KẾT HỢP ENTITY:
        - "danh sách thuốc còn tồn kho thấp / ít" → is_low_stock=True (hoặc dùng inventory_alerts low_stock).
        - "danh sách thuốc còn tồn kho nhiều / thuốc nào tồn kho nhiều nhất" → KHÔNG dùng tool này (trả theo lô, không tổng hợp); dùng analytics_report_tool(report_type="TopStockMedicine").
        - "thuốc nào bán chạy nhất" → KHÔNG dùng tool này; dùng analytics_report_tool(report_type="TopSellingMedicine").
        - "thuốc X còn hàng không / giá thuốc X" → tool này với keyword=ten thuoc.
        Chỉ truyền tham số nếu người dùng yêu cầu rõ ràng.
        """
        payload = clean_payload({
            "Keyword": keyword,
            "MedicineId": medicine_id,
            "CategoryId": category_id,
            "ManufacturerId": manufacturer_id,
            "WareHouseId": warehouse_id,
            "IsExpiringSoon": is_expiring_soon,
            "InStock": in_stock,
            "OutOfStock": out_of_stock,
            "IsLowStock": is_low_stock,
            "StockThreshold": stock_threshold,
            "MinPrice": min_price,
            "MaxPrice": max_price,
            "Count": count,
        })
        data = await api_post(
            "/api/ai/inventory/search",
            json_body=payload,
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        items = get_field(data, "Items", []) if isinstance(data, dict) else data
        if not items:
            return "Khong tim thay thuoc nao phu hop."
        return json.dumps(items, ensure_ascii=False, default=str)

    @tool
    async def inventory_alerts(
        alert_type: str,
        threshold: Optional[float] = None,
    ) -> str:
        """THUỐC & TỒN KHO - Xem cam canh bao ton kho cua chi nhanh.
        Loai canh bao (alert_type):
        - 'expiring': thuoc sap het han (3-6 thang toi)
        - 'low_stock': thuoc ton kho thap
        - 'out_of_stock': thuoc het hang
        - 'in_stock': thuoc con hang
        Args:
            alert_type: Loai canh bao: expiring | low_stock | out_of_stock | in_stock
            threshold: Nguong ton thap (mac dinh 10, chi dung khi alert_type='low_stock')
        """
        presets = {
            "expiring": {"IsExpiringSoon": True},
            "low_stock": {"IsLowStock": True, "StockThreshold": threshold},
            "out_of_stock": {"OutOfStock": True},
            "in_stock": {"InStock": True},
        }
        if alert_type not in presets:
            return "alert_type khong hop le. Chi chap nhan: expiring | low_stock | out_of_stock | in_stock"

        payload = clean_payload(presets[alert_type])
        data = await api_post(
            "/api/ai/inventory/search",
            json_body=payload,
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        items = get_field(data, "Items", []) if isinstance(data, dict) else data
        if not items:
            return "Khong co canh bao nao cho loai nay."
        return json.dumps(items, ensure_ascii=False, default=str)

    @tool
    async def allocate_fefo(medicine_id: int, quantity: float) -> str:
        """Phan bo lo thuoc theo phuong phap FEFO (First-Expired-First-Out) cho 1 thuoc.
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

    return [search_inventory, inventory_alerts, allocate_fefo]