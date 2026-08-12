using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface IStockTakeRepository
    {
        Task<StockTake?> GetByIdAsync(long id);
        Task<List<StockTake>> GetAllAsync(int skip, int take, long warehouseId);
        Task<int> CountAsync(long warehouseId);
        Task AddAsync(StockTake entity);
        void Update(StockTake entity);
        Task<WareHouse?> GetWarehouseByIdAsync(long warehouseId);
        Task<Dictionary<long, Batch>> GetBatchesByIdsAsync(IEnumerable<long> batchIds);
        Task SaveChangesAsync();
    }
}
