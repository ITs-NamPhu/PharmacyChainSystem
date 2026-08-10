namespace PharmacyManagement.Models
{
	public class InvoiceItem
	{
		public long InvoiceItemID { get; set; }
        public long InvoiceID { get; set; }
        public long BatchID { get; set; }

        // đơn vị mua
        public long UnitID { get; set; }
        public string UnitName {get;set;}
        
        // số lượng theo đơn vị mua
        public decimal Quantity { get; set; }

        // chuyển số lượng đơn vị mua -> đơn vị tính cơ sở
        public decimal ConversionFactor { get; set; }

        // số lượng theo quy đổi tại mỗi thời điểm
        public decimal BaseQuantity { get; set; }

        public decimal UnitPrice { get; set; }

        public Invoice? Invoice { get; set; }
        public Batch? Batch { get; set; }
        public Unit? Unit { get; set; }
    }
}
