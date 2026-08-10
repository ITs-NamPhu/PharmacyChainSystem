namespace PharmacyManagement.Models
{
	public class PurchaseReturnItem
	{
		public long PurchaseReturnItemID { get; set; }
        public long PurchaseReturnID { get; set; }
        public long BatchID { get; set; }
        public decimal Quantity { get; set; }

        public PurchaseReturn? Purchase { get; set; }
        public Batch? Batch { get; set; }
	}
}
