namespace PharmacyManagement.DTOs.StockAdjustment
{
    public class StockAdjustmentDetailResponse
    {
        public long StockAdjustmentID { get; set; }
        public long StockTakeID { get; set; }
        public long WarehouseID { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public long UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public List<StockAdjustmentItemDetailResponse> Items { get; set; } = new();
    }

    public class StockAdjustmentItemDetailResponse
    {
        public long StockAdjustmentItemID { get; set; }
        public long BatchID { get; set; }
        public long? StockTakeItemID { get; set; }
        public long MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public decimal AdjustQuantity { get; set; }
        public string? ReasonCode { get; set; }
    }
}
