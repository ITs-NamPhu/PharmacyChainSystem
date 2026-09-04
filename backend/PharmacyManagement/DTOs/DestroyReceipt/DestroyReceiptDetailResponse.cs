namespace PharmacyManagement.DTOs.DestroyReceipt
{
    public class DestroyReceiptDetailResponse
    {
        public long DestroyReceiptID { get; set; }
        public long WarehouseID { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public long? StockTakeID { get; set; }
        public long UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public bool IsFromStockTake { get; set; }
        public List<DestroyReceiptItemDetailResponse> Items { get; set; } = new();
    }

    public class DestroyReceiptItemDetailResponse
    {
        public long DestroyReceiptItemID { get; set; }
        public long BatchID { get; set; }
        public long? StockTakeItemID { get; set; }
        public long MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public string? ReasonCode { get; set; }
    }
}
