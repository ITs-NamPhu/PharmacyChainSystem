namespace PharmacyManagement.DTOs.Warehouse
{
    public class WarehouseResponse
    {
        public long WarehouseID { get; set; }
        public long BranchID { get; set; }
        public string WarehouseName { get; set; } = "";
        public long? WarehouseType { get; set; }
    }
}
