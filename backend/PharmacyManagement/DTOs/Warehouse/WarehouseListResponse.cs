namespace PharmacyManagement.DTOs.Warehouse
{
    public class WarehouseListResponse
    {
        public int NumRecords { get; set; }
        public float TotalPage { get; set; }
        public List<WarehouseResponse> Warehouses { get; set; } = new();
    }
}
