namespace PharmacyManagement.DTOs.GoodsReceipt
{
    public class CreateGoodsReceiptRequest
    {
        public long SupplierID { get; set; }
        public string? Note { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime ReceiptDate { get; set; }
        public List<CreateGoodsReceiptItemRequest> Items { get; set; } = new();
    }

    public class UpdateGoodsReceiptRequest
    {
        public long SupplierID { get; set; }
        public string? Note { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime ReceiptDate { get; set; }
        public List<UpdateGoodsReceiptItemRequest> Items { get; set; } = new();
    }
}
