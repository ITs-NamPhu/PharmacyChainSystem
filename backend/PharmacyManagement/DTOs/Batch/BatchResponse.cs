namespace PharmacyManagement.DTOs.Batch
{
    public class BatchByMedicineResponse
    {
        public long BatchID { get; set; }
        public decimal QuantityInStock { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal UnitCost { get; set; }
        public string UnitName { get; set; } = string.Empty;
    }

    public class FefoResultResponse
    {
        public long MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public decimal TotalQuantityRequested { get; set; }
        public bool Fulfilled { get; set; }
        public List<AllocatedBatchDto> AllocatedBatches { get; set; } = new();
    }

    public class AllocatedBatchDto
    {
        public long BatchID { get; set; }
        public decimal QuantityAllocated { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal QuantityInStock { get; set; }
    }
}
