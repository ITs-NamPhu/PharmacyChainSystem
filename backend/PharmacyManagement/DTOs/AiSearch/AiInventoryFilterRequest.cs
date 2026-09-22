namespace PharmacyManagement.DTOs.AiSearch
{
    public class AiInventoryFilterRequest : AiBaseFilterRequest
    {
        public long? MedicineId { get; set; }
        public string? Keyword { get; set; }
        public long? CategoryId { get; set; }
        public long? ManufacturerId { get; set; }
        public long? WareHouseId { get; set; }
        public bool? IsExpiringSoon { get; set; }
        public bool? InStock { get; set; }
        public bool? OutOfStock { get; set; }
        public bool? IsLowStock { get; set; }
        public decimal? StockThreshold { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public long? BranchId { get; set; }
    }

    public class AiInventoryItem
    {
        public long MedicineId { get; set; }
        public string MedicineName { get; set; } = "";
        public string? UnitName { get; set; }
        public string? CategoryName { get; set; }
        public string? ManufacturerName { get; set; }
        public long BatchID { get; set; }
        public string BatchName { get; set; } = "";
        public decimal QuantityReceived { get; set; }
        public decimal QuantityInStock { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Price { get; set; }
        public string? WarehouseName { get; set; }
        public string? BranchName { get; set; }
    }
}