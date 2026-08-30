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

        // Số tiền mặt khách trả ngay khi mua (phần còn lại là nợ)
        public decimal PaidAmount { get; set; }

        // Số tiền khách muốn trả bằng ví (gửi > 0 nếu đồng ý dùng ví)
        public decimal? UseWalletAmount { get; set; }

        public BatchSelectionMode Mode { get; set; } = BatchSelectionMode.FEFO;
        public List<CreateInvoiceItemRequest> InvoiceItems { get; set; } = new();
    }

    public class UpdateInvoiceRequest
    {
        public long CustomerID { get; set; }
        public string? Note { get; set; }
        public long? CreatedByUserID { get; set; }
        public decimal PaidAmount { get; set; }
        public BatchSelectionMode Mode { get; set; } = BatchSelectionMode.FEFO;
        public List<UpdateInvoiceItemRequest> InvoiceItems { get; set; } = new();
    }
}
