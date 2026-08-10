namespace PharmacyManagement.Models
{
    public class GoodsReceiptItem
    {
        public long GoodsReceiptItemID { get; set; }
        public long GoodsReceiptID { get; set; }
        public long MedicineID { get; set; }

        // đơn vị nhập
        public long UnitID { get; set; }

        public string UnitName { get; set; }


        // số lượng theo đơn vị nhập
        public decimal Quantity { get; set; }

        // chuyển số lượng đơn vị nhập -> đơn vị tính cơ sở
        public decimal ConversionFactor { get; set; }

        // đơn giá nhập
        public decimal UnitCost { get; set; }

        public GoodsReceipt? GoodsReceipt { get; set; }
        public Medicine? Medicine { get; set; }
        public ICollection<Batch>? Batch { get; set; }
        public Unit? Unit { get; set; }

    }
}
