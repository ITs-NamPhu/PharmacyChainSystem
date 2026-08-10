using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace PharmacyManagement.Models
{
	public class ManuFacturer
	{
		public long ManufacturerID { get; set; }
        [StringLength(255)]
        public String ManufacturerName { get; set; }

        public ICollection<Medicine> Medicine { get; set; }
    }
}
