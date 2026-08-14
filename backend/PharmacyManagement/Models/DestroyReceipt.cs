using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PharmacyManagement.Models
{
    [Index(nameof(StockTakeID), IsUnique = true)]
    public class DestroyReceipt
	{
		public long DestroyReceiptID { get; set; }
        public long WarehouseID { get; set; }
        public long UserID { get; set; }
        public long StockTakeID { get; set; }

        [StringLength(255)]
        public String Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public WareHouse? WareHouse { get; set; }
        public User? User { get; set; }
        public StockTake? StockTake { get; set; }
        public ICollection<DestroyReceiptItem>? DestroyReceiptItem { get; set; }
    }
}
