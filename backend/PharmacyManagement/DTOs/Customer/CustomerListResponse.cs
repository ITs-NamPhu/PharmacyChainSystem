namespace PharmacyManagement.DTOs.Customer
{
    public class CustomerListResponse
    {
        public int NumRecords { get; set; }
        public float TotalPage { get; set; }
        public List<CustomerResponse> Customers { get; set; } = new();
    }
}
