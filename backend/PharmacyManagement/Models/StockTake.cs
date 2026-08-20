using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyManagement.Models
{
	public class StockTake
	{
		public long StockTakeID { get; set; }
        public long WarehouseID { get; set; }
        public long UserID { get; set; }
        public DateTime CreatedAt { get; set; }

        [StringLength(255)]
        public String Note { get; set; }
        public StockTakeResult IsBalance { get; set; }
        public StockTakeStatus Status { get; set; }
        public bool IsAdjust { get; set; }
        public bool IsDestroy { get; set; }
        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public WareHouse? WareHouse { get; set; }
        public User? User { get; set; }
		public ICollection<StockTakeItem>? StockTakeItem { get;set; }
    }

    public enum StockTakeStatus
    {
        Draft,
        Completed,
        Cancelled
    }

    public enum StockTakeResult
    {
        Balanced,
        Difference
    }
}