using PharmacyManagement.DTOs.InvoiceItem;

namespace PharmacyManagement.DTOs.Invoice
{
    public class InvoiceDetailResponse
    {
        public long InvoiceID { get; set; }
        public long CustomerID { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; } = string.Empty;
        public long UserID { get; set; }
        public List<InvoiceItemDetailResponse> InvoiceItems { get; set; } = new();
    }
}
