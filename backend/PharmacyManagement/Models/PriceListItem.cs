namespace PharmacyManagement.Models
{
	public class PriceListItem
	{
		public long PriceListItemID { get; set; }
        public long PriceListID { get; set; }
        public long MedicineID { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal WholesalePrice { get; set; }

        public PriceList? PriceList { get; set; }
        public Medicine? Medicine { get; set; }

    }
}