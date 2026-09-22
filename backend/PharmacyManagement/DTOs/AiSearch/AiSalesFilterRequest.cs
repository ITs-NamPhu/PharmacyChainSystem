namespace PharmacyManagement.DTOs.AiSearch
{
    public class AiSalesFilterRequest : AiBaseFilterRequest
    {
        public long? InvoiceId { get; set; }
        public long? CustomerId { get; set; }
        public long? UserId { get; set; }
        public string? CustomerPhone { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? HasReturnedItems { get; set; }
        public Models.PaymentStatus? PaymentStatus { get; set; }
        public decimal? MinTotal { get; set; }
        public decimal? MaxTotal { get; set; }
        public long? BranchId { get; set; }
    }

    public class AiSalesItem
    {
        public long InvoiceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Outstanding { get; set; }
        public Models.PaymentStatus PaymentStatus { get; set; }
        public string? SellerName { get; set; }
        public bool HasReturnedItems { get; set; }
    }
}