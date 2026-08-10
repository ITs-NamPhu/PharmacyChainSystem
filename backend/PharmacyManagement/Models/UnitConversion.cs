using System.Numerics;

namespace PharmacyManagement.Models
{
	public class UnitConversion
	{
		public long UnitConversionID { get; set; }
        public long UnitID { get; set; }
        public long MedicineID { get; set; }
        public decimal Factor { get; set; }

        public Medicine? Medicine { get; set; }
        public Unit? Unit { get; set; }

    }
}