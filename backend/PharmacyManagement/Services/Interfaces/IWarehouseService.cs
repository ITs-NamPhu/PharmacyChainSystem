using PharmacyManagement.DTOs.Warehouse;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IWarehouseService
    {
        Task<WarehouseResponse> CreateAsync(CreateWarehouseRequest request, long branchId);
        Task<WarehouseResponse> UpdateAsync(long id, UpdateWarehouseRequest request);
        Task DeleteAsync(long id);
        Task<WarehouseResponse?> GetByIdAsync(long id);
        Task<WarehouseListResponse> GetAllAsync(int page, int count);
        Task<WarehouseListResponse> GetByBranchAsync(long branchId, int page, int count);
        Task<WarehouseResponse?> GetByBranchIdAsync(long branchId);
        Task<WarehouseBatchListResponse> GetBatchesByWarehouseAsync(long warehouseId, int page, int count);
    }
}
