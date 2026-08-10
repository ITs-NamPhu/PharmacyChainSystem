using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace PharmacyManagement.Models
{
	public class Branch
	{
        public long BranchID { get; set; }

        [StringLength(255)]
        public String BranchName { get; set; }

        [StringLength(10)]
        public String Phone { get; set; }

        [StringLength(255)]
        public String Address { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Boolean IsActive { get; set; }
        public long PriceListID { get; set; }

        public ICollection<UserBranch>? UserBranch { get; set; }
        public PriceList? PriceList { get; set; }
        public ICollection<WareHouse>? WareHouse { get; set; }
        public ICollection<GoodsReceipt>? GoodsReceipt { get;set; }
        public ICollection<Invoice>? Invoice { get; set; }

    }
}
