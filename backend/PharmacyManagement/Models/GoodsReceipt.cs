using System.ComponentModel.DataAnnotations;

namespace PharmacyManagement.Models
{
    public class GoodsReceipt
    {
        public long GoodsReceiptID { get; set; }
        public long ReceiptNumber { get; set; }
        public long SupplierID { get; set; }
        public long BranchID { get; set; }
        public long UserID { get; set; }

        public DateTime ReceiptDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }

        [StringLength(255)]
        public String Note { get; set; }

        public Supplier? Supplier { get; set; }
        public Branch? Branch { get; set; }
        public User? User { get; set; }

        public ICollection<GoodsReceiptItem>? GoodsReceiptItem { get; set; }
    }
}
