using System.ComponentModel.DataAnnotations;
using System.Numerics;
using PharmacyManagement.share;

namespace PharmacyManagement.Models
{
	public class Customer : ISoftDelete
	{
		public long CustomerID { get; set; }

        // Số dư ví khách hàng (mặc định 0), dùng để trừ khi khách thanh toán bằng ví
        public decimal WalletBalance { get; set; }

        [StringLength(255)]
        public String CustomerName { get; set; }

        [StringLength(10)]
        public String? Phone { get; set; }

        [StringLength(255)]
        public String? Address { get; set; }

        [StringLength(255)]
        public String? Email { get; set; }

        public long CustomerTypeID { get; set; }

        public CustomerType? CustomerType { get; set; }
        public ICollection<Invoice>? Invoice { get; set; }
        public ICollection<SalesReturn>? SalesReturn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
