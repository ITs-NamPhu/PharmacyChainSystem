using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface IInvoiceRepository
    {
        IQueryable<Invoice> GetQuery();

        Task<Invoice?> GetByIdAsync(long id);
        Task<List<Invoice>> GetAllAsync(int skip, int take, long branchId);
        Task<int> CountAsync(long branchId);
        Task AddAsync(Invoice entity);
        void Update(Invoice entity);
        void Delete(Invoice entity);
        Task SaveChangesAsync();

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        Task<List<Batch>> GetBatchesByMedicineAsync(long medicineID, long branchID);
        Task<Dictionary<long, List<Batch>>> GetBatchesByMedicineIdsAsync(IEnumerable<long> medicineIds, long branchId);
        Task<Batch?> GetBatchByIdAsync(long batchID);
        Task<Dictionary<long, Batch>> GetBatchesByIdsAsync(IEnumerable<long> batchIds);
        Task<bool> IsCustomerExistsAsync(long customerId);
        Task<bool> IsMedicineExistsAsync(long medicineId);
        Task<List<Medicine>> GetMedicinesByIdsAsync(IEnumerable<long> medicineIds);
        Task<bool> TryDecrementStockAsync(long batchId, decimal quantity);
        Task IncrementStockAsync(long batchId, decimal quantity);
        void RemoveInvoiceItems(IEnumerable<InvoiceItem> items);
    }
}
