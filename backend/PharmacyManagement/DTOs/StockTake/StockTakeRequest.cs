namespace PharmacyManagement.DTOs.StockTake
{
    public class CreateStockTakeRequest
    {
        public long WarehouseID { get; set; }
        public string? Note { get; set; }
        public List<CreateStockTakeItemRequest> Items { get; set; } = new();
    }

    public class CreateStockTakeItemRequest
    {
        public long BatchID { get; set; }
        public decimal ActualQuantity { get; set; }
        public bool IsAdjust { get; set; }
        public bool IsDestroy { get; set; }
    }
}
