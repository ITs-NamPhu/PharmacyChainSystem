using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface IGoodsReceiptRepository
    {
        Task<GoodsReceipt?> GetByIdAsync(long id);
        Task<List<GoodsReceipt>> GetAllAsync(int skip, int take, long branchId);
        Task<int> CountAsync(long branchId);
        Task<long> GetNextReceiptNumberAsync();
        Task AddAsync(GoodsReceipt entity);
        void Update(GoodsReceipt entity);
        void Delete(GoodsReceipt entity);
        void RemoveGoodsReceiptItems(IEnumerable<GoodsReceiptItem> items);
        Task<GoodsReceiptItem?> GetItemByIdAsync(long itemId);
        Task<Batch?> GetBatchByIdAsync(long batchId);
        Task SaveChangesAsync();
    }
}
