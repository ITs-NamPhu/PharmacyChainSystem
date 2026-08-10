using PharmacyManagement.DTOs.Warehouse;
using PharmacyManagement.Models;

namespace PharmacyManagement.Mappers
{
    public static class WarehouseMapper
    {
        public static WareHouse ToEntity(this CreateWarehouseRequest request, long branchId)
        {
            return new WareHouse
            {
                BranchID = branchId,
                WarehouseName = request.WarehouseName,
                WarehouseType = request.WarehouseType
            };
        }

        public static void ApplyTo(this UpdateWarehouseRequest request, WareHouse entity)
        {
            entity.WarehouseName = request.WarehouseName;
            entity.WarehouseType = request.WarehouseType;
        }

        public static WarehouseResponse ToResponse(this WareHouse entity)
        {
            return new WarehouseResponse
            {
                WarehouseID = entity.WarehouseID,
                BranchID = entity.BranchID,
                WarehouseName = entity.WarehouseName,
                WarehouseType = entity.WarehouseType
            };
        }

        public static List<WarehouseResponse> ToResponseList(this List<WareHouse> entities)
        {
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
