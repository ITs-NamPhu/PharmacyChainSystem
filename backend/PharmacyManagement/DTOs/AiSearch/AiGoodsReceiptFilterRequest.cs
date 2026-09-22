namespace PharmacyManagement.DTOs.AiSearch
{
    public class AiGoodsReceiptFilterRequest : AiBaseFilterRequest
    {
        public long? GoodsReceiptId { get; set; }
        public string? Keyword { get; set; }
        public long? SupplierId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Status { get; set; }
        public decimal? MinTotal { get; set; }
        public decimal? MaxTotal { get; set; }
        public long? BranchId { get; set; }
    }

    public class AiGoodsReceiptItem
    {
        public long GoodsReceiptId { get; set; }
        public long ReceiptNumber { get; set; }
        public string SupplierName { get; set; } = "";
        public string UserName { get; set; } = "";
        public DateTime ReceiptDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string Note { get; set; } = "";
        public string Status { get; set; } = "";
        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }
}