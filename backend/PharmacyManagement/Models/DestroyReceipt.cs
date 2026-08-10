using System.ComponentModel.DataAnnotations;

namespace PharmacyManagement.Models
{
	public class DestroyReceipt
	{
		public long DestroyReceiptID { get; set; }
        public long WarehouseID { get; set; }
        public long UserID { get; set; }

        [StringLength(255)]
        public String Note { get; set; }
        public DateTime CreatedAt { get; set; }

        public WareHouse? WareHouse { get; set; }
        public User? User { get; set; }
        public ICollection<DestroyReceiptItem>? DestroyReceiptItem { get; set; }
    }
}
