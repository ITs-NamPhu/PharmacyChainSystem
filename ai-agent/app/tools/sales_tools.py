import json
from typing import Optional
from langchain_core.tools import tool

from app.tools.base import api_post, clean_payload, get_field
from app.models.chat import AuthContext

_PAYMENT_STATUS_MAP = {"debt": 0, "paid": 1}


def create_sales_tools(auth: AuthContext):
    @tool
    async def search_sales(
        invoice_id: Optional[int] = None,
        customer_id: Optional[int] = None,
        user_id: Optional[int] = None,
        customer_phone: Optional[str] = None,
        from_date: Optional[str] = None,
        to_date: Optional[str] = None,
        has_returned_items: Optional[bool] = None,
        payment_status: Optional[str] = None,
        min_total: Optional[float] = None,
        max_total: Optional[float] = None,
        count: Optional[int] = None,
    ) -> str:
        """
        BÁN HÀNG - Tra cứu HÓA ĐƠN (danh sách dòng hóa đơn riêng lẻ).
        Kết quả gồm: InvoiceId, CreatedAt, CustomerName, CustomerPhone, TotalAmount, PaidAmount, Outstanding, PaymentStatus, SellerName (User.FullName), HasReturnedItems.
        Args:
            invoice_id: ID hóa đơn
            customer_id: ID khách hàng
            user_id: ID nhân viên bán hàng (người tạo hóa đơn)
            customer_phone: Số điện thoại khách hàng
            from_date: Ngày bắt đầu (dạng YYYY-MM-DD)
            to_date: Ngày kết thúc (dạng YYYY-MM-DD)
            has_returned_items: True = chỉ hóa đơn có phát sinh trả hàng
            payment_status: Trang thai thanh toán: 'debt' (con no) | 'paid' (da tra het)
            min_total: ổng tiền tối thiểu
            max_total: ổng tiền tối đa
            count: Số kết quả tối đa (mặc định 20)

        KẾT HỢP ENTITY:
        - "user nào lập hóa đơn có id=123" → truyền invoice_id=123, đọc SellerName từ kết quả.
        - "hóa đơn của khách hàng có số điện thoại 09..." → customer_phone.
        - "danh sách hóa đơn còn nợ" → payment_status='debt'.
        - "tổng doanh thu / thuốc bán chạy / khách hàng mua nhiều nhất / user lập nhiều hóa đơn nhất / giá trị mua lớn nhất" → KHÔNG dùng tool này (vì trả dòng thô, có phân trang); dùng analytics_report_tool.
        Chỉ truyền tham số nếu người dùng yêu cầu rõ ràng.
        """
        payment_status_val = None
        if payment_status:
            payment_status_val = _PAYMENT_STATUS_MAP.get(payment_status.lower())
            if payment_status_val is None:
                return "payment_status khong hop le. Chi chap nhan: 'debt' | 'paid'"

        payload = clean_payload({
            "InvoiceId": invoice_id,
            "CustomerId": customer_id,
            "UserId": user_id,
            "CustomerPhone": customer_phone,
            "FromDate": from_date,
            "ToDate": to_date,
            "HasReturnedItems": has_returned_items,
            "PaymentStatus": payment_status_val,
            "MinTotal": min_total,
            "MaxTotal": max_total,
            "Count": count,
        })
        data = await api_post(
            "/api/ai/sales/search",
            json_body=payload,
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        items = get_field(data, "Items", []) if isinstance(data, dict) else data
        if not items:
            return "Khong tim thay hoa don nao phu hop."
        return json.dumps(items, ensure_ascii=False, default=str)

    return [search_sales]