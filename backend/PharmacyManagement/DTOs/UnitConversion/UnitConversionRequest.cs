namespace PharmacyManagement.DTOs.UnitConversion
{
    public class CreateUnitConversionRequest
    {
        public long UnitID { get; set; }
        public long MedicineID { get; set; }
        public decimal Factor { get; set; }
    }

    public class UpdateUnitConversionRequest
    {
        public decimal Factor { get; set; }
    }
}
