namespace PharmacyManagement.DTOs.CustomerDebtSummary
{
    public class CustomerDebtSummaryResponse
    {
        public long CustomerDebtSummaryID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Increase { get; set; }
        public decimal Paid { get; set; }
        public decimal ClosingBalance { get; set; }
        public bool IsLocked { get; set; }
        public long CustomerID { get; set; }
        public string? CustomerName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }

    public class CustomerDebtSummaryListResponse
    {
        public int NumRecords { get; set; }
        public int TotalPage { get; set; }
        public List<CustomerDebtSummaryResponse> Items { get; set; } = new();
    }
}
