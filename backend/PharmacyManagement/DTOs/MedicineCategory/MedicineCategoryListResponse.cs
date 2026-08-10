namespace PharmacyManagement.DTOs.MedicineCategory
{
    public class MedicineCategoryListResponse
    {
        public int NumRecords { get; set; }
        public float TotalPage { get; set; }
        public List<MedicineCategoryResponse> MedicineCategories { get; set; } = new();
    }
}
