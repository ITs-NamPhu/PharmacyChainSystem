using System.ComponentModel.DataAnnotations;

namespace PharmacyManagement.Models
{
	public class PriceList
	{
		public long PriceListID { get; set; }

        [StringLength(255)]
        public String PriceListName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ICollection<PriceListItem>? PriceListItem { get; set; }
        public ICollection<Branch>? Branch { get; set; }
    }
}
