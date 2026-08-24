using System.ComponentModel.DataAnnotations;
using System.Numerics;
using PharmacyManagement.share;

namespace PharmacyManagement.Models
{

	public class User : ISoftDelete
	{
		public long UserID { get; set; }

        [StringLength(50)]
        public String UserName { get; set; }
        public String PasswordHash { get; set; }

        [StringLength(255)]
        public String FullName { get; set; }
        [StringLength(10)]
        public String Phone { get; set; }
        [StringLength(255)]
        public String? Address { get; set; }
        [StringLength(255)]
        public String? Email { get; set; }

        public Boolean IsActive { get; set; }

        public int FailedLoginCount { get; set; }

        public DateTime? LockoutEnd { get; set; }

        public DateTime? LastFailedLogin { get; set; }



        public ICollection<UserBranch>? UserBranch { get; set; } = new List<UserBranch>();
        public ICollection<GoodsReceipt>? GoodsReceipt { get; set; }
        public ICollection<Invoice>? Invoice { get; set; }
        public ICollection<PurchaseReturn>? PurchaseReturn { get; set; }
        public ICollection<SalesReturn>? SalesReturn { get; set; }
        public ICollection<DestroyReceipt>? DestroyReceipt { get; set; }
        public ICollection<StockAdjustment>? StockAdjustment { get; set; }
        public ICollection<StockTake>? StockTake { get; set; }
        public ICollection<InventoryTransaction>? InventoryTransaction { get; set; }
        public ICollection<AuditLog>? AuditLog { get; set; }
        public ICollection<RefreshToken>? RefreshToken { get; set; }

        public bool IsDeleted { get; set; }

    }
}