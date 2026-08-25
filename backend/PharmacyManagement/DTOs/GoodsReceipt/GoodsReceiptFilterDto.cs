using PharmacyManagement.share;

namespace PharmacyManagement.DTOs.GoodsReceipt
{
    public class GoodsReceiptFilterDto : BaseFilterDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long? SupplierID { get; set; }
        public decimal? MinTotalAmount { get; set; }
        public decimal? MaxTotalAmount { get; set; }
    }
}
