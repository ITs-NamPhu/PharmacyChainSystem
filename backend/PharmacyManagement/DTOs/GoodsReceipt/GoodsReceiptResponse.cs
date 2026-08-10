namespace PharmacyManagement.DTOs.GoodsReceipt
{
    public class GoodsReceiptResponse
    {
        public long GoodsReceiptID { get; set; }
        public long ReceiptNumber { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime ReceiptDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
