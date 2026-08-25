using PharmacyManagement.share;

namespace PharmacyManagement.DTOs.Batch
{
    public enum ExpiryStatus
    {
        All = 0,
        NearExpiry3Months = 1,
        NearExpiry6Months = 2,
        Expired = 3
    }

    public class BatchFilterDto : BaseFilterDto
    {
        public ExpiryStatus ExpiryStatus { get; set; } = ExpiryStatus.All;
        public long? WarehouseID { get; set; }
    }
}
