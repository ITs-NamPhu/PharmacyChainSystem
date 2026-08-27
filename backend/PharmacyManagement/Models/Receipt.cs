namespace PharmacyManagement.Models
{
	public class Receipt
	{
		public long ReceiptID { get; set; }
        public long CustomerID { get; set; }
        public long UserID { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }

        public Customer? Customer { get; set; }
        public User? User { get; set; }
    }
}
