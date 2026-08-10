namespace PharmacyManagement.DTOs.UnitConversion
{
    public class UnitConversionListResponse
    {
        public int NumRecords { get; set; }
        public float TotalPage { get; set; }
        public List<UnitConversionResponse> UnitConversions { get; set; } = new();
    }
}
