namespace PharmacyManagement.DTOs.Warehouse
{
    public class CreateWarehouseRequest
    {
        public string WarehouseName { get; set; } = "";
        public long? WarehouseType { get; set; }
    }

    public class UpdateWarehouseRequest
    {
        public string WarehouseName { get; set; } = "";
        public long? WarehouseType { get; set; }
    }
}
