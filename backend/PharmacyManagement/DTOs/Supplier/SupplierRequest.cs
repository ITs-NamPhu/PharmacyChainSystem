namespace PharmacyManagement.DTOs.Supplier
{
    public class CreateSupplierRequest
    {
        public string SupplierName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
    }

    public class UpdateSupplierRequest
    {
        public string SupplierName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
    }
}
