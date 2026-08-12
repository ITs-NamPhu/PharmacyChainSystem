namespace PharmacyManagement.DTOs.StockTake
{
    public class StockTakeDetailResponse
    {
        public long StockTakeID { get; set; }
        public long WarehouseID { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public long UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Note { get; set; } = string.Empty;
        public string IsBalance { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsAdjusted { get; set; }
        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public List<StockTakeItemDetailResponse> Items { get; set; } = new();
    }

    public class StockTakeItemDetailResponse
    {
        public long StockTakeItemID { get; set; }
        public long BatchID { get; set; }
        public long MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public decimal SystemQuantity { get; set; }
        public decimal ActualQuantity { get; set; }
        public decimal DifferenceQuantity { get; set; }
    }
}
