using System.ComponentModel.DataAnnotations;
using System.Numerics;
using PharmacyManagement.share;

namespace PharmacyManagement.Models
{
	public class ManuFacturer : ISoftDelete
	{
		public long ManufacturerID { get; set; }
        [StringLength(255)]
        public String ManufacturerName { get; set; }

        public ICollection<Medicine> Medicine { get; set; }

        public bool IsDeleted { get; set; }
    }
}
