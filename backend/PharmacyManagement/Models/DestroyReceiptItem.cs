namespace PharmacyManagement.Models
{
	public class DestroyReceiptItem
	{
		public long DestroyReceiptItemID { get; set; }
        public long DestroyReceiptID { get; set; }
        public long BatchID { get; set; }
        public decimal Quantity { get; set; }

        public DestroyReceipt? DestroyReceipt { get; set; }
        public Batch? Batch { get; set; }

    }
}
