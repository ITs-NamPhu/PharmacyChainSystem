namespace PharmacyManagement.DTOs.AiSearch
{
    public class AiAnalyticsRequest
    {
        // "TotalRevenue" | "RevenueTrend" | "TopSellingMedicine" | "TopStockMedicine"
        // | "LowStockMedicine" | "TopCustomer" | "TopCustomerByInvoiceCount" | "TopUserByInvoices"
        public string ReportType { get; set; } = "";

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int? TopN { get; set; }
    }

    public class AiAnalyticsResponse
    {
        public string ReportType { get; set; } = "";

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public List<AiAnalyticsRow> Items { get; set; } = new();
    }

    public class AiAnalyticsRow
    {
        public long EntityId { get; set; }

        public string EntityName { get; set; } = "";

        // Metric dùng để xếp hạng / giá trị chính (doanh thu, số lượng, tồn kho, số hóa đơn...)
        public decimal PrimaryMetric { get; set; }

        // Metric bổ sung (số hóa đơn, doanh thu, số lô...)
        public decimal SecondaryMetric { get; set; }

        public DateTime? Date { get; set; }
    }
}