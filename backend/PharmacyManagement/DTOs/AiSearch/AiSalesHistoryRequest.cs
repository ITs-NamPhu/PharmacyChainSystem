namespace PharmacyManagement.DTOs.AiSearch
{
    public class AiSalesHistoryRequest
    {
        public long? MedicineId { get; set; }
        public string? Keyword { get; set; }
        public long? BranchId { get; set; }

        private int _days = 30;
        public int Days
        {
            get => _days;
            set => _days = value < 1 ? 1 : (value > 90 ? 90 : value);
        }
    }

    public class AiSalesHistoryDto
    {
        public long ProductId { get; init; }
        public string? ProductName { get; init; }
        public long? BranchId { get; init; }
        public int PeriodDays { get; init; }
        public string? UnitName { get; init; }
        public SalesSummaryDto Summary { get; init; } = new();

        // Dictionary date -> quantity: tiết kiệm token hơn mảng object cho AI đọc
        public Dictionary<string, decimal> DailyRecords { get; init; } = new();
    }

    public record SalesSummaryDto
    {
        public decimal TotalQuantitySold { get; init; }
        public decimal AverageDaily { get; init; }
        public string SalesTrend { get; init; } = "STABLE";
    }
}
