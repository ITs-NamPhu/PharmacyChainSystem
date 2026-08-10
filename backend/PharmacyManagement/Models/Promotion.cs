using System.ComponentModel.DataAnnotations;

namespace PharmacyManagement.Models
{
	public class Promotion
	{
		public long PromotionID { get; set; }

        [StringLength(255)]
        public String PromotionName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ICollection<PromotionItem>? PromotionItem { get; set; }

    }
}
