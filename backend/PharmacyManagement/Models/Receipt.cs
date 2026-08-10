namespace PharmacyManagement.Models
{
	public class Receipt
	{
		public long ReceiptID { get; set; }
        public long ReceiptNumber { get; set; }
        public long CustomerID { get; set; }
        public decimal TotalAmount { get; set; }

        public Customer? Customer { get; set; }
    }
}