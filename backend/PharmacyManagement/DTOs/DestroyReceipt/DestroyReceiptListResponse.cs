namespace PharmacyManagement.DTOs.DestroyReceipt
{
    public class DestroyReceiptListResponse
    {
        public int NumRecords { get; set; }
        public float TotalPage { get; set; }
        public List<DestroyReceiptResponse> DestroyReceipts { get; set; } = new();
    }
}
