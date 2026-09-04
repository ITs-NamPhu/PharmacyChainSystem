namespace PharmacyManagement.DTOs.DestroyReceipt
{
    public class CreateDestroyReceiptRequest
    {
        public long? StockTakeID { get; set; }
        public long WarehouseID { get; set; }
        public string? Note { get; set; }
        public List<CreateDestroyReceiptItemRequest> Items { get; set; } = new();
    }

    public class CreateDestroyReceiptItemRequest
    {
        public long BatchID { get; set; }
        public long? StockTakeItemID { get; set; }
        public decimal Quantity { get; set; }
        public string? ReasonCode { get; set; }
    }

    public class UpdateDestroyReceiptRequest
    {
        public long WarehouseID { get; set; }
        public string? Note { get; set; }
        public List<UpdateDestroyReceiptItemRequest> Items { get; set; } = new();
    }

    public class UpdateDestroyReceiptItemRequest
    {
        public long BatchID { get; set; }
        public decimal Quantity { get; set; }
        public string? ReasonCode { get; set; }
    }
}
