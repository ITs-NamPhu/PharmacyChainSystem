namespace PharmacyManagement.DTOs.StockTake
{
    public class StockTakeListResponse
    {
        public int NumRecords { get; set; }
        public float TotalPage { get; set; }
        public List<StockTakeResponse> StockTakes { get; set; } = new();
    }
}
