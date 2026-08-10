namespace PharmacyManagement.DTOs.Manufacturer
{
    public class ManufacturerListResponse
    {
        public int NumRecords { get; set; }
        public float TotalPage { get; set; }
        public List<ManufacturerResponse> Manufacturers { get; set; } = new();
    }
}
