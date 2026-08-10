namespace PharmacyManagement.DTOs.CustomerType
{
    public class CustomerTypeListResponse
    {
        public int NumRecords { get; set; }
        public float TotalPage { get; set; }
        public List<CustomerTypeResponse> CustomerTypes { get; set; } = new();
    }
}
