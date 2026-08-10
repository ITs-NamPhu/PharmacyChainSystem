namespace PharmacyManagement.Models
{
	public class PurchaseReturn
	{
		public long PurchaseReturnID { get; set; }
        public long SupplierID { get; set; }
        public long UserID { get; set; }
        public DateTime CreatedAt { get; set; }
		public String Note { get; set; }

        public Supplier? Supplier { get; set; }
        public User? User { get; set; }
		public ICollection<PurchaseReturnItem>? PurchaseReturnItem { get; set; }
    }
}
