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
        Task<StockTakeItem?> GetStockTakeItemAsync(long stockTakeItemId);
        Task<Batch?> GetBatchByIdAsync(long batchId);
        Task<bool> TryAdjustStockAsync(long batchId, decimal quantity);
        Task SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
