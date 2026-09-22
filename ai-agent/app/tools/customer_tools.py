import json
from typing import Optional
from langchain_core.tools import tool

from app.tools.base import api_post, clean_payload, get_field
from app.models.chat import AuthContext


def create_customer_tools(auth: AuthContext):
    @tool
    async def search_customers(
        keyword: Optional[str] = None,
        customer_id: Optional[int] = None,
        customer_type_id: Optional[int] = None,
        has_active_debt: Optional[bool] = None,
        min_wallet_balance: Optional[float] = None,
        max_wallet_balance: Optional[float] = None,
        count: Optional[int] = None,
    ) -> str:
        
        """
        KHÁCH HÀNG - Tra cứu KHÁCH HÀNG và công nợ (danh sách khách hàng riêng lẻ).
        Kết quả gồm: CustomerId, CustomerName, Phone, Address, Email, CustomerTypeName, WalletBalance, DebtAmount.
        Args:
            keyword: Tên hoặc số điện thoại khách hàng
            customer_id: ID cụ thể của khách hàng
            customer_type_id: ID nhóm khách hàng
            has_active_debt: True = chỉ lấy khách hàng đang còn nợ
            min_wallet_balance: lọc khách hàng có ví tích điểm >= giá trị này
            max_wallet_balance: lọc khách hàng có ví tích điểm <= giá trị này
            count: Số kết quả tối đa (mặc định 20)

        GIỚI HẠN / KẾT HỢP ENTITY:
        - Tool này KHÔNG trả số hóa đơn hay giá trị mua hàng của khách hàng.
        - "khách hàng nào có giá trị mua hàng lớn nhất" hoặc "khách hàng có nhiều hóa đơn nhất" → KHÔNG dùng tool này; dùng analytics_report_tool(report_type="TopCustomer" hoặc "TopCustomerByInvoiceCount").
        - "thông tin khách hàng X" / "khách hàng nào còn nợ" → dùng tool này (keyword / has_active_debt=True).
        Chỉ truyền tham số nếu người dùng yêu cầu rõ ràng.
        """
        
        
        payload = clean_payload({
            "Keyword": keyword,
            "CustomerId": customer_id,
            "CustomerTypeId": customer_type_id,
            "HasActiveDebt": has_active_debt,
            "MinWalletBalance": min_wallet_balance,
            "MaxWalletBalance": max_wallet_balance,
            "Count": count,
        })
        
        data = await api_post(
            "/api/ai/customer/search",
            json_body=payload,
            token=auth.token,
            branch_id=auth.branch_id,
        )
        
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        
        
        items = get_field(data, "Items", []) if isinstance(data, dict) else data
        if not items:
            return "Khong tim thay khach hang nao phu hop."
        return json.dumps(items, ensure_ascii=False, default=str)

    return [search_customers]