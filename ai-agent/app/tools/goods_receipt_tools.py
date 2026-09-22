import json
from typing import Optional
from langchain_core.tools import tool

from app.tools.base import api_post, clean_payload, get_field
from app.models.chat import AuthContext


def create_goods_receipt_tools(auth: AuthContext):
    @tool
    async def search_goods_receipts(
        goods_receipt_id: Optional[int] = None,
        keyword: Optional[str] = None,
        supplier_id: Optional[int] = None,
        from_date: Optional[str] = None,
        to_date: Optional[str] = None,
        status: Optional[str] = None,
        min_total: Optional[float] = None,
        max_total: Optional[float] = None,
        count: Optional[int] = None,
    ) -> str:
        """
        NHẬP HÀNG - Tra cứu phiếu nhập hàng (GoodsReceipt).
        Kết quả gồm: GoodsReceiptId, ReceiptNumber, SupplierName, UserName (người lập phiếu), ReceiptDate, TotalAmount, PaidAmount, Status.

        Args:
            goods_receipt_id: ID phiếu nhập cụ thể
            keyword: Tên nhà cung cấp hoặc số phiếu nhập
            supplier_id: ID nhà cung cấp
            from_date: Ngày bắt đầu (YYYY-MM-DD)
            to_date: Ngày kết thúc (YYYY-MM-DD)
            status: Trạng thái phiếu: PENDING | APPROVED | COMPLETE | REJECTED
            min_total: Tổng tiền phiếu nhập tối thiểu
            max_total: Tổng tiền phiếu nhập tối đa
            count: Số kết quả tối đa (mặc định 20)

        Ví dụ:
        - "danh sách phiếu nhập hàng 7 ngày qua" → from_date/to_date.
        - "nhà cung cấp X nhập hàng tháng này" → keyword="X" + from_date/to_date.
        Chỉ truyền tham số nếu người dùng yêu cầu rõ ràng.
        """
        payload = clean_payload({
            "GoodsReceiptId": goods_receipt_id,
            "Keyword": keyword,
            "SupplierId": supplier_id,
            "FromDate": from_date,
            "ToDate": to_date,
            "Status": status,
            "MinTotal": min_total,
            "MaxTotal": max_total,
            "Count": count,
        })
        data = await api_post(
            "/api/ai/goodsreceipt/search",
            json_body=payload,
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        items = get_field(data, "Items", []) if isinstance(data, dict) else data
        if not items:
            return "Khong tim thay phieu nhap hang nao phu hop."
        return json.dumps(items, ensure_ascii=False, default=str)

    return [search_goods_receipts]