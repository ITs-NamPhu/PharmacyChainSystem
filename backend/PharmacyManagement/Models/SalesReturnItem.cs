namespace PharmacyManagement.Models
{
	public class SalesReturnItem
	{
		public long SalesReturnItemID { get; set; }
        public long SalesReturnID { get; set; }
        public long BatchID { get; set; }
        public decimal Quantity { get; set; }

        public SalesReturn? SalesReturn { get; set; }
        public Batch? Batch { get; set; }
    }
}
