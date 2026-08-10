using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace PharmacyManagement.Models
{
	public class Customer
	{
		public long CustomerID { get; set; }

        [StringLength(255)]
        public String CustomerName { get; set; }

        [StringLength(10)]
        public String? Phone { get; set; }

        [StringLength(255)]
        public String? Address { get; set; }

        public long CustomerTypeID { get; set; }

        public CustomerType? CustomerType { get; set; }
        public ICollection<Invoice>? Invoice { get; set; }
        public ICollection<SalesReturn>? SalesReturn { get; set; }

    }
}
