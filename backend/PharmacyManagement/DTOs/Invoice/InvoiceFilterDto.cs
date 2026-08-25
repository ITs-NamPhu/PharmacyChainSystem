using PharmacyManagement.share;

namespace PharmacyManagement.DTOs.Invoice
{
    public class InvoiceFilterDto : BaseFilterDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long? CustomerID { get; set; }
        public long? UserID { get; set; }
        public decimal? MinTotalAmount { get; set; }
        public decimal? MaxTotalAmount { get; set; }
    }
}
