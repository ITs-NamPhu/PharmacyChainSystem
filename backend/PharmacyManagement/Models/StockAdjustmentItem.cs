using System.ComponentModel.DataAnnotations;

namespace PharmacyManagement.Models
{
	public class StockAdjustmentItem
	{
		public long StockAdjustmentItemID { get; set; }
        public long StockAdjustmentID { get; set; }
        public long BatchID { get; set; }
        public long? StockTakeItemID { get; set; }
        public decimal AdjustQuantity { get; set; }

        [StringLength(255)]
        public String? ReasonCode { get; set; }

        public StockAdjustment? StockAdjustment { get; set; }
        public Batch? Batch { get; set; }
        public StockTakeItem? StockTakeItem { get; set; }

    }
}