namespace PharmacyManagement.DTOs.Receipt
{
    public class CreateReceiptRequest
    {
        public long CustomerID { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class ReceiptResponse
    {
        public long ReceiptID { get; set; }
        public long CustomerID { get; set; }
        public string? CustomerName { get; set; }
        public long UserID { get; set; }
        public string? UserName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class ReceiptListResponse
    {
        public int NumRecords { get; set; }
        public int TotalPage { get; set; }
        public List<ReceiptResponse> Items { get; set; } = new();
    }
}
