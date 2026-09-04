using PharmacyManagement.share;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyManagement.Models
{
    public class StockTake : ISoftDelete
    {
        public long StockTakeID { get; set; }
        public long WarehouseID { get; set; }
        public long UserID { get; set; }
        public DateTime CreatedAt { get; set; }

        [StringLength(255)]
        public String Note { get; set; }
        public StatusTicket Status { get; set; }

        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public bool IsDeleted { get; set; }

        public WareHouse? WareHouse { get; set; }
        public User? User { get; set; }
        public ICollection<StockTakeItem>? StockTakeItem { get; set; }
        public StockAdjustment? StockAdjustment { get; set; }
        public DestroyReceipt? DestroyReceipt { get; set; }
    }
}
