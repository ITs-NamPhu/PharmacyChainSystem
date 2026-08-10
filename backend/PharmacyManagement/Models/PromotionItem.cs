namespace PharmacyManagement.Models
{
	public class PromotionItem
	{
		public long PromotionItemID { get; set; }
        public long PromotionID { get; set; }
        public long MedicineID { get; set; }
        public decimal DiscountValue { get; set; }

        public Promotion? Promotion { get; set; }
        public Medicine? Medicine { get; set; }
    }
}
