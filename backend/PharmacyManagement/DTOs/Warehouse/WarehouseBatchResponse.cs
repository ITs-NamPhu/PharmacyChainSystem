namespace PharmacyManagement.DTOs.Warehouse
{
    public class WarehouseBatchListResponse
    {
        public int NumRecords { get; set; }
        public float TotalPage { get; set; }
        public List<WarehouseBatchResponse> Batches { get; set; } = new();
    }

    public class WarehouseBatchResponse
    {
        public long BatchID { get; set; }
        public long MedicineID { get; set; }
        public string MedicineName { get; set; } = "";
        public string UnitName { get; set; } = "";
        public decimal UnitCost { get; set; }
        public decimal QuantityReceived { get; set; }
        public decimal QuantityInStock { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string? Note { get; set; }
    }
}
