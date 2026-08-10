namespace PharmacyManagement.DTOs.CustomerType
{
    public class CreateCustomerTypeRequest
    {
        public string TypeName { get; set; } = "";
        public decimal DiscountPercent { get; set; }
    }

    public class UpdateCustomerTypeRequest
    {
        public string TypeName { get; set; } = "";
        public decimal DiscountPercent { get; set; }
    }
}
