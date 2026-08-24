using System.ComponentModel.DataAnnotations;
using System.Numerics;
using PharmacyManagement.share;

namespace PharmacyManagement.Models
{
	public class Supplier : ISoftDelete
	{
		public long SupplierID { get; set; }

        [StringLength(255)]
		public String SupplierName { get; set; }
        [StringLength(10)]
        public String Phone { get; set; }
        [StringLength(255)]
        public String Email { get; set; }
        [StringLength(255)]
        public String Address { get; set; }

        public ICollection<GoodsReceipt>? GoodsReceipt { get; set; }
        public ICollection<Supplier>? Suplier { get; set; }

        public bool IsDeleted { get; set; }

    }
}
