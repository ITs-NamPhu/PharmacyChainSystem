using PharmacyManagement.DTOs.DestroyReceipt;
using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface IDestroyReceiptRepository
    {
        Task<DestroyReceipt?> GetByIdAsync(long id);
        Task<List<DestroyReceiptResponse>> GetAllAsync(int skip, int take, long warehouseId);
        Task<int> CountAsync(long warehouseId);
        Task AddAsync(DestroyReceipt entity);
        void Delete(DestroyReceipt entity);
        Task<WareHouse?> GetWarehouseByIdAsync(long warehouseId);
        Task<bool> HasDestroyReceiptAsync(long stockTakeId);
        Task<StockTake?> GetStockTakeByIdAsync(long stockTakeId);
        Task<Dictionary<long, Batch>> GetBatchesByIdsAsync(IEnumerable<long> batchIds);
        Task<Dictionary<long, StockTakeItem>> GetStockTakeItemsByIdsAsync(IEnumerable<long> stockTakeItemIds);
        Task SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
