namespace PharmacyManagement.Models
{
	public class StockAdjustmentItem
	{
		public long StockAdjustmentItemID { get; set; }
        public long StockAdjustmentID { get; set; }
        public long BatchID { get; set; }
        public decimal AdjustQuantity { get; set; }

        public StockAdjustment? StockAdjustment { get; set; }
        public Batch? Batch { get; set; }

    }
}