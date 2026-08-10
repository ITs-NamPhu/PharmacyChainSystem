namespace PharmacyManagement.DTOs
{
    public class BatchAllocation
    {
        public long BatchID { get; set; }
        public decimal Quantity { get; set; }
        public PharmacyManagement.Models.Batch? Batch { get; set; }
    }
}
