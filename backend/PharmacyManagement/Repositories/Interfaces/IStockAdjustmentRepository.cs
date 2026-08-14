using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface IStockAdjustmentRepository
    {
        Task<StockAdjustment?> GetByIdAsync(long id);
        Task<List<StockAdjustment>> GetAllAsync(int skip, int take, long warehouseId);
        Task<int> CountAsync(long warehouseId);
        Task AddAsync(StockAdjustment entity);
        Task<bool> HasAdjustmentAsync(long stockTakeId);
        Task<StockTake?> GetStockTakeByIdAsync(long stockTakeId);
        Task<Dictionary<long, Batch>> GetBatchesByIdsAsync(IEnumerable<long> batchIds);
        Task SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
