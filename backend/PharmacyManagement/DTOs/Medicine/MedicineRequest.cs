namespace PharmacyManagement.DTOs.Medicine
{
    public class CreateMedicineRequest
    {
        public string MedicineName { get; set; } = "";
        public decimal DefaultRetailPrice { get; set; }
        public decimal DefaultWholesalePrice { get; set; }
        public decimal VATPercent { get; set; }
        public long CategoryID { get; set; }
        public long ManufacturerID { get; set; }
        public long BaseUnitID { get; set; }
    }

    public class UpdateMedicineRequest
    {
        public string MedicineName { get; set; } = "";
        public decimal DefaultRetailPrice { get; set; }
        public decimal DefaultWholesalePrice { get; set; }
        public decimal VATPercent { get; set; }
        public long? CategoryID { get; set; }
        public long? ManufacturerID { get; set; }
        public long? BaseUnitID { get; set; }
    }
}
