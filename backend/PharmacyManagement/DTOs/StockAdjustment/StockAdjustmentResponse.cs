namespace PharmacyManagement.DTOs.StockAdjustment
{
    public class StockAdjustmentResponse
    {
        public long StockAdjustmentID { get; set; }
        public long? StockTakeID { get; set; }
        public long WarehouseID { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public long UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public bool IsFromStockTake { get; set; }
        public int ItemCount { get; set; }
    }
}
