using System.ComponentModel.DataAnnotations;
using PharmacyManagement.share;

namespace PharmacyManagement.Models
{
	public class Invoice : ISoftDelete
	{
		public long InvoiceID { get; set; }
        public long CustomerID { get; set; }
        public long BranchID { get; set; }
        public long UserID { get; set; }

        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }

        [StringLength(255)]
        public String Note { get; set; }

        public ICollection<InvoiceItem>? InvoiceItem { get; set; }
        public Customer? Customer { get; set; }
        public Branch? Branch { get; set; }
        public User? User { get; set; }

        public bool IsDeleted { get; set; }

    }
}