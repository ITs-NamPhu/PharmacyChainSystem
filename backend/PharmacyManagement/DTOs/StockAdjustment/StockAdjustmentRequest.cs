namespace PharmacyManagement.DTOs.StockAdjustment
{
    public class CreateStockAdjustmentRequest
    {
        public long? StockTakeID { get; set; }
        public long WarehouseID { get; set; }
        public string? Note { get; set; }
        public List<CreateStockAdjustmentItemRequest> Items { get; set; } = new();
    }

    public class CreateStockAdjustmentItemRequest
    {
        public long BatchID { get; set; }
        public long? StockTakeItemID { get; set; }
        public decimal AdjustQuantity { get; set; }
        public string? ReasonCode { get; set; }
    }

    public class UpdateStockAdjustmentRequest
    {
        public long StockAdjustmentID { get; set; }
        public long WarehouseID { get; set; }
        public string? Note { get; set; }
        public List<UpdateStockAdjustmentItemRequest> Items { get; set; } = new();
    }

    public class UpdateStockAdjustmentItemRequest
    {
        public long StockAdjustmentItemID { get; set; }
        public long BatchID { get; set; }
        public decimal AdjustQuantity { get; set; }
        public string? ReasonCode { get; set; }
    }
}
