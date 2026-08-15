namespace PharmacyManagement.Models
{

	public class StockTakeItem
	{
		public long StockTakeItemID { get; set; }
        public long StockTakeID { get; set; }
        public long BatchID { get; set; }
        public decimal SystemQuantity { get; set; }
        public decimal ActualQuantity { get; set; }
        public decimal DifferenceQuantity { get; set; }
        public bool IsAdjust { get; set; }
        public bool IsDestroy { get; set; }

        public StockTake? StockTake { get; set; }
        public Batch? Batch { get; set; }

    }
}
