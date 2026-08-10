using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace PharmacyManagement.Models
{
	public class Medicine
	{
		public long MedicineID { get; set; }

        [StringLength(255)]
        public String MedicineName { get; set; }

        public decimal DefaultRetailPrice { get; set; }
        public decimal DefaultWholesalePrice { get; set; }
        public decimal VATPercent { get; set; }

        public long CategoryID { get; set; }
        public long ManufacturerID { get; set; }
        // đơn vị tính mặc định
        public long BaseUnitID { get; set; }


        public MedicineCategory? MedicineCategory { get; set; }
        public ManuFacturer? ManuFacturer { get; set; }
        public Unit? Unit { get; set; }
        public ICollection<PromotionItem>? PromotionItem { get; set; }
        public ICollection<PriceListItem>? PriceListItem { get; set; }
    }
}
