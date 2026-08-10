namespace PharmacyManagement.DTOs.GoodsReceipt
{
    public class GoodsReceiptListResponse
    {
        public int NumRecords { get; set; }
        public float TotalPage { get; set; }
        public List<GoodsReceiptResponse> GoodsReceipts { get; set; } = new();
    }
}
