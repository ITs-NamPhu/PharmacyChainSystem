namespace PharmacyManagement.DTOs.Medicine
{
    public class MedicineResponse
    {
        public long MedicineID { get; set; }
        public string MedicineName { get; set; } = "";
        public decimal DefaultRetailPrice { get; set; }
        public decimal DefaultWholesalePrice { get; set; }
        public decimal VATPercent { get; set; }
        public long CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public long ManufacturerID { get; set; }
        public string? ManufacturerName { get; set; }
        public long BaseUnitID { get; set; }
        public string? UnitName { get; set; }
        public decimal TotalStock { get; set; }
    }
}
