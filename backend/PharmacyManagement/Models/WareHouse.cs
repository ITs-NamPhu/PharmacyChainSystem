using System.ComponentModel.DataAnnotations;

namespace PharmacyManagement.Models
{
	public class WareHouse
	{
        public long WarehouseID { get; set; }
        public long BranchID { get; set; }
        [StringLength(255)]
        public String WarehouseName { get; set; }
        public long? WarehouseType { get; set; }

        public Branch? Branch { get; set; }
        public ICollection<DestroyReceipt>? DestroyReceipt { get; set; }
        public ICollection<StockAdjustment>? StockAdjustment { get; set; }
        public ICollection<StockTake>? StockTake { get; set; }

    }
}
