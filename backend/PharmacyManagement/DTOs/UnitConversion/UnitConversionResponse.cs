namespace PharmacyManagement.DTOs.UnitConversion
{
    public class UnitConversionResponse
    {
        public long UnitConversionID { get; set; }
        public long UnitID { get; set; }
        public long MedicineID { get; set; }
        public decimal Factor { get; set; }
    }

    public class UnitConversion_MedicineResponse
    {
        public long UnitConversionID { get; set; }
        public long UnitID { get; set; }
        public long MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;

        public long BaseUnitID { get; set; }

        public string BaseUnitName { get; set; } = string.Empty;

        public decimal Factor { get; set; }

    }
}
