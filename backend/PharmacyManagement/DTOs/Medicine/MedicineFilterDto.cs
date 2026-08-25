using PharmacyManagement.share;

namespace PharmacyManagement.DTOs.Medicine
{
    public enum StockStatus
    {
        All = 0,
        InStock = 1,
        LowStock = 2,
        OutOfStock = 3
    }

    public class MedicineFilterDto : BaseFilterDto
    {
        public long? CategoryID { get; set; }
        public long? ManufacturerID { get; set; }
        public StockStatus StockStatus { get; set; } = StockStatus.All;
        public decimal? StockThreshold { get; set; }
    }
}
