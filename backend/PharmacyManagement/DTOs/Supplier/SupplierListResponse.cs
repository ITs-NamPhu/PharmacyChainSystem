namespace PharmacyManagement.DTOs.Supplier
{
    public class SupplierListResponse
    {
        public int NumRecords { get; set; }
        public float TotalPage { get; set; }
        public List<SupplierResponse> Suppliers { get; set; } = new();
    }
}
