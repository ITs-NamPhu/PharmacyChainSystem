namespace PharmacyManagement.DTOs.GoodsReceipt
{
    public class CreateGoodsReceiptItemRequest
    {
        public long MedicineID { get; set; }
        public long UnitID { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }

        public DateTime ManufactureDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }

    public class UpdateGoodsReceiptItemRequest
    {
        public long? GoodsReceiptItemID { get; set; }
        public long MedicineID { get; set; }
        public long UnitID { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
