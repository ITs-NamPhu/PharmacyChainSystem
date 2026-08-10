using System.Numerics;

namespace PharmacyManagement.Models
{
	public class CustomerType
	{
		public long CustomerTypeID { get; set; }
        public String TypeName { get; set; }
        public decimal DiscountPercent { get; set; }

        public ICollection<Customer>? Customer { get; set; }
	}
}
