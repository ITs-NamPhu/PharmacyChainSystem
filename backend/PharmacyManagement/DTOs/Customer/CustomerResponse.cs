namespace PharmacyManagement.DTOs.Customer
{
    public class CustomerResponse
    {
        public long CustomerID { get; set; }
        public string CustomerName { get; set; } = "";
        public string? Phone { get; set; } = "";
        public string? Address { get; set; } = "";
        public string? Email { get; set; } = "";
        public long CustomerTypeID { get; set; }
        public string? CustomerTypeName { get; set; }
    }
}
