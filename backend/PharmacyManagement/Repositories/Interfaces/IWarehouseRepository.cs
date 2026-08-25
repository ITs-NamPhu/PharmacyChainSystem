using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface IWarehouseRepository
    {
        Task<bool> IsBranchExistsAsync(long branchId);
        Task<WareHouse?> GetByIdAsync(long id);
        Task<List<WareHouse>> GetAllAsync(int skip, int take);
        Task<List<WareHouse>> GetByBranchAsync(long branchId, int skip, int take);
        Task<WareHouse?> GetByBranchIdAsync(long branchId);
        Task<int> CountAsync();
        Task<int> CountByBranchAsync(long branchId);
        Task AddAsync(WareHouse entity);
        void Update(WareHouse entity);
        void Delete(WareHouse entity);
        Task SaveChangesAsync();

        Task<List<Batch>> GetBatchesByWarehouseAsync(long warehouseId, int skip, int take);
        Task<int> CountBatchesByWarehouseAsync(long warehouseId);

        IQueryable<Batch> GetBatchQuery();
    }
}
