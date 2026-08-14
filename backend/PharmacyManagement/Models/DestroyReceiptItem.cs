using System.ComponentModel.DataAnnotations;

namespace PharmacyManagement.Models
{
	public class DestroyReceiptItem
	{
		public long DestroyReceiptItemID { get; set; }
        public long DestroyReceiptID { get; set; }
        public long BatchID { get; set; }
        public long? StockTakeItemID { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }

        [StringLength(255)]
        public String? ReasonCode { get; set; }

        public DestroyReceipt? DestroyReceipt { get; set; }
        public Batch? Batch { get; set; }
        public StockTakeItem? StockTakeItem { get; set; }

    }
}
