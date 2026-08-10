namespace PharmacyManagement.DTOs.CustomerType
{
    public class CustomerTypeResponse
    {
        public long CustomerTypeID { get; set; }
        public string TypeName { get; set; } = "";
        public decimal DiscountPercent { get; set; }
    }
}
