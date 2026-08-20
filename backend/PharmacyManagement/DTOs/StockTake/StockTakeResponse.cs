namespace PharmacyManagement.DTOs.StockTake
{
    public class StockTakeResponse
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
        public bool IsAdjust { get; set; }
        public bool IsDestroy { get; set; }
        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int ItemCount { get; set; }
    }
}
