import json
from datetime import datetime
from typing import Optional
from langchain_core.tools import tool

from app.tools.base import api_post, clean_payload, get_field
from app.models.chat import AuthContext


def _build_description() -> str:
    today = datetime.now().strftime("%Y-%m-%d")
    return f"""CÔNG CỤ BÁO CÁO THỐNG KÊ (Dùng khi hỏi TỔNG, DOANH THU, TOP, NHIỀU NHẤT, BÁN CHẠY NHẤT, CAO NHẤT).
        Hôm nay: {today}.

        report_type hợp lệ:
        - "TotalRevenue": tổng doanh thu (toàn thời gian hoặc theo from_date/to_date).
        - "RevenueTrend": doanh thu theo từng ngày trong khoảng thời gian (nếu không truyền ngày, mặc định 30 ngày gần nhất).
        - "TopSellingMedicine": top thuốc bán chạy nhất (PrimaryMetric = số lượng bán, SecondaryMetric = doanh thu). VD "thuốc nào bán chạy nhất".
        - "TopStockMedicine": top thuốc tồn kho nhiều nhất (PrimaryMetric = tổng lượng tồn kho).
        - "LowStockMedicine": danh sách thuốc còn tồn kho thấp (PrimaryMetric = lượng tồn kho).
        - "TopCustomer": khách hàng có giá trị mua hàng lớn nhất (PrimaryMetric = tổng tiền, SecondaryMetric = số hóa đơn).
        - "TopCustomerByInvoiceCount": khách hàng có nhiều hóa đơn nhất (PrimaryMetric = số hóa đơn).
        - "TopUserByInvoices": nhân viên lập nhiều hóa đơn nhất (PrimaryMetric = số hóa đơn, SecondaryMetric = tổng tiền).

        Args:
            report_type: Loại báo cáo (bắt buộc, xem danh sách ở trên).
            from_date: Ngày bắt đầu dạng YYYY-MM-DD (không bắt buộc).
            to_date: Ngày kết thúc dạng YYYY-MM-DD (không bắt buộc).
            top_n: Số dòng kết quả mong muốn cho báo cáo TOP (mặc định 10).

        Ví dụ:
        - "Nếu doanh thu 7 ngày gần nhất" → report_type="RevenueTrend", from_date=ngày bắt đầu, to_date={today}.
        - "khách hàng nào có giá trị mua hàng lớn nhất" → report_type="TopCustomer", top_n=1.
        - "thuốc nào bán chạy nhất" → report_type="TopSellingMedicine", top_n=1.
        
        Lưu ý:
        - khi hỏi về doanh thu thì kiểm tra role không phải là admin hoặc manage_supply thì gọi tool với branch từ header, 
           nếu role admin hoặc manage_supply trong request không chỉ định chi nhánh cụ thể thì gọi tool với branch_id = None (tức là tất cả chi nhánh), nếu có chỉ định chi nhánh cụ thể thì gọi tool với branch_id = giá trị chỉ định.
        """


def create_analytics_tools(auth: AuthContext):
    @tool(description=_build_description())
    async def analytics_report_tool(
        report_type: str,
        from_date: Optional[str] = None,
        to_date: Optional[str] = None,
        top_n: Optional[int] = None,
    ) -> str:
        """Báo cáo thống kê doanh thu / top / nhiều nhất."""
        valid_types = {
            "TotalRevenue", "RevenueTrend", "TopSellingMedicine",
            "TopStockMedicine", "LowStockMedicine", "TopCustomer",
            "TopCustomerByInvoiceCount", "TopUserByInvoices",
        }
        if report_type not in valid_types:
            return (
                f"report_type '{report_type}' khong hop le. "
                f"Chi chap nhan: {', '.join(sorted(valid_types))}"
            )

        payload = clean_payload({
            "ReportType": report_type,
            "FromDate": from_date,
            "ToDate": to_date,
            "TopN": top_n,
        })
        data = await api_post(
            "/api/ai/analytics/report",
            json_body=payload,
            token=auth.token,
            branch_id=auth.branch_id,
        )
        if isinstance(data, dict) and "error" in data:
            return data["error"]
        if not isinstance(data, dict) or not get_field(data, "Items"):
            return "Khong tim thay du lieu phu hop cho bao cao nay."
        return json.dumps(data, ensure_ascii=False, default=str)

    return [analytics_report_tool]