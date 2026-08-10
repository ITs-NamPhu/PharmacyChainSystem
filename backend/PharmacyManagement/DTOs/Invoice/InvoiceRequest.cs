using PharmacyManagement.DTOs.InvoiceItem;

namespace PharmacyManagement.DTOs.Invoice
{
    public enum BatchSelectionMode
    {
        FEFO,
        Manual
    }

    public class CreateInvoiceRequest
    {
        public long CustomerID { get; set; }
        public string? Note { get; set; }
        public long? CreatedByUserID { get; set; }
        public BatchSelectionMode Mode { get; set; } = BatchSelectionMode.FEFO;
        public List<CreateInvoiceItemRequest> InvoiceItems { get; set; } = new();
    }

    public class UpdateInvoiceRequest
    {
        public long CustomerID { get; set; }
        public string? Note { get; set; }
        public long? CreatedByUserID { get; set; }
        public BatchSelectionMode Mode { get; set; } = BatchSelectionMode.FEFO;
        public List<UpdateInvoiceItemRequest> InvoiceItems { get; set; } = new();
    }
}
