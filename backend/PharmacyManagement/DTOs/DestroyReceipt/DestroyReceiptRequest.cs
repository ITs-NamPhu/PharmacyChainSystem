namespace PharmacyManagement.DTOs.DestroyReceipt
{
    public class CreateDestroyReceiptRequest
    {
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
}
