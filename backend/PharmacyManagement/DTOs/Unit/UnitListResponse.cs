namespace PharmacyManagement.DTOs.Unit
{
    public class UnitListResponse
    {
        public int NumRecords { get; set; }
        public float TotalPage { get; set; }
        public List<UnitResponse> Units { get; set; } = new();
    }
}
