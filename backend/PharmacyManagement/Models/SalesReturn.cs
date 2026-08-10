using System.ComponentModel.DataAnnotations;

namespace PharmacyManagement.Models
{
	public class SalesReturn
	{
		public long SalesReturnID { get; set; }
        public long CustomerID { get; set; }
        public long UserID { get; set; }

        public DateTime CreatedAt { get; set; }
        [StringLength(255)]
        public String Note { get; set; }

        public Customer? Customer { get; set; }
        public User? User { get; set; }
        public ICollection<SalesReturnItem>? SalesReturnItem { get; set; }
    }
}
