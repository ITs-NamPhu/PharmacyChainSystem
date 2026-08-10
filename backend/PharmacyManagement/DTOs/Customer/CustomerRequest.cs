namespace PharmacyManagement.DTOs.Customer
{
    public class CreateCustomerRequest
    {
        public string CustomerName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public long CustomerTypeID { get; set; }
    }

    public class UpdateCustomerRequest
    {
        public string CustomerName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public long CustomerTypeID { get; set; }
    }
}
