namespace PharmacyManagement.DTOs.StockAdjustment
{
    public class CreateStockAdjustmentRequest
    {
        public long StockTakeID { get; set; }
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
}
