using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using PharmacyManagement.share;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace PharmacyManagement.Models
{
    [Index(nameof(StockTakeID), IsUnique = true)]
    public class StockAdjustment : ISoftDelete
    {
        public long StockAdjustmentID { get; set; }
        public long WarehouseID { get; set; }
        public long UserID { get; set; }
        public long? StockTakeID { get; set; }

        [StringLength(255)]
        public String? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public StatusTicket Status { get; set; }
        public bool IsDeleted { get; set; }
        public WareHouse? WareHouse { get; set; }
        public User? User { get; set; }
        public StockTake? StockTake { get; set; }
        public ICollection<StockAdjustmentItem>? StockAdjustmentItem { get; set; }
    }
}
