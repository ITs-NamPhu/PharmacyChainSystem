namespace PharmacyManagement.DTOs.Medicine
{
    public class MedicineListResponse
    {
        public int NumRecords { get; set; }
        public float TotalPage { get; set; }
        public List<MedicineResponse> Medicines { get; set; } = new();
    }
}
