namespace PharmacyManagement.DTOs.Invoice
{
    public class InvoiceResponse
    {
        public long InvoiceID { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public long CustomerID { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public Models.PaymentStatus PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
