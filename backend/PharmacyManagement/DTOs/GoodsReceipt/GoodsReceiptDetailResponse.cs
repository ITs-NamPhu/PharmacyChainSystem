namespace PharmacyManagement.DTOs.GoodsReceipt
{
    public class GoodsReceiptDetailResponse
    {
        public long GoodsReceiptID { get; set; }
        public long ReceiptNumber { get; set; }
        public long SupplierID { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public long UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime ReceiptDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string Note { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public List<GoodsReceiptItemDetailResponse> Items { get; set; } = new();
    }

    public class GoodsReceiptItemDetailResponse
    {
        public long GoodsReceiptItemID { get; set; }
        public long MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public long UnitID { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal ConversionFactor { get; set; }
        public decimal UnitCost { get; set; }
        public decimal QuantityReceived { get; set; }
        public decimal QuantityInStock { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
