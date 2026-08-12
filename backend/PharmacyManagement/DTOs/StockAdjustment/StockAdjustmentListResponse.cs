namespace PharmacyManagement.DTOs.StockAdjustment
{
    public class StockAdjustmentListResponse
    {
        public int NumRecords { get; set; }
        public float TotalPage { get; set; }
        public List<StockAdjustmentResponse> StockAdjustments { get; set; } = new();
    }
}
