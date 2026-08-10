namespace PharmacyManagement.DTOs.Invoice
{
    public class InvoiceListResponse
    {
        public int NumRecords { get; set; }
        public float TotalPage { get; set; }
        public List<InvoiceResponse> Invoices { get; set; } = new();
    }
}
