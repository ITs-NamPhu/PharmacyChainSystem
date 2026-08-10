using System.Numerics;

namespace PharmacyManagement.Models
{
	public class MedicineCategory
	{
		public long MedicineCategoryID { get; set; }
        public String CategoryName { get; set; }

		public ICollection<Medicine> Medicine { get; set; }
    }
}
