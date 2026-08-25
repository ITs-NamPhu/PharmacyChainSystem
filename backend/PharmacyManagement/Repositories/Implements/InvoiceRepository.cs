using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly PharmacySystemDbContext _context;
        private IDbContextTransaction? _transaction;

        public InvoiceRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public IQueryable<Invoice> GetQuery()
        {
            return _context.Invoice.AsNoTracking();
        }

        public async Task<Invoice?> GetByIdAsync(long id)
        {
            return await _context.Invoice
                .Include(i => i.Customer)
                .Include(i => i.User)
                .Include(i => i.InvoiceItem)
                    .ThenInclude(ii => ii.Batch)
                        .ThenInclude(b => b!.GoodsReceiptItem)
                            .ThenInclude(gri => gri!.Medicine)
                .FirstOrDefaultAsync(i => i.InvoiceID == id);
        }

        public async Task<List<Invoice>> GetAllAsync(int skip, int take, long branchId)
        {
            return await _context.Invoice
                .Include(i => i.Customer)
                .Include(i => i.User)
                .Where(i => i.BranchID == branchId)
                .OrderByDescending(i => i.CreatedAt)
                .Skip(skip).Take(take)
                .ToListAsync();
        }

        public async Task<int> CountAsync(long branchId)
        {
            return await _context.Invoice
                .Where(i => i.BranchID == branchId)
                .CountAsync();
        }

        public async Task AddAsync(Invoice entity)
        {
            await _context.Invoice.AddAsync(entity);
        }

        public void Update(Invoice entity)
        {
            _context.Invoice.Update(entity);
        }

        public void Delete(Invoice entity)
        {
            _context.Invoice.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null) return;
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null) return;
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task<List<Batch>> GetBatchesByMedicineAsync(long medicineID, long branchID)
        {
            return await _context.Batch
                .Include(b => b.GoodsReceiptItem)
                    .ThenInclude(gri => gri!.GoodsReceipt)
                .Where(b => b.GoodsReceiptItem!.MedicineID == medicineID
                          && b.GoodsReceiptItem!.GoodsReceipt!.BranchID == branchID
                          && b.QuantityInStock > 0)
                .OrderBy(b => b.ExpiryDate)
                .ToListAsync();
        }

        public async Task<Dictionary<long, List<Batch>>> GetBatchesByMedicineIdsAsync(IEnumerable<long> medicineIds, long branchId)
        {
            var ids = medicineIds.ToList();
            if (ids.Count == 0)
                return new Dictionary<long, List<Batch>>();

            var batches = await _context.Batch
                .Include(b => b.GoodsReceiptItem)
                    .ThenInclude(gri => gri!.GoodsReceipt)
                .Where(b => ids.Contains(b.GoodsReceiptItem!.MedicineID)
                          && b.GoodsReceiptItem!.GoodsReceipt!.BranchID == branchId
                          && b.QuantityInStock > 0)
                .OrderBy(b => b.ExpiryDate)
                .ToListAsync();

            return batches
                .GroupBy(b => b.GoodsReceiptItem!.MedicineID)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public async Task<Batch?> GetBatchByIdAsync(long batchID)
        {
            return await _context.Batch
                .Include(b => b.GoodsReceiptItem)
                    .ThenInclude(gri => gri!.GoodsReceipt)
                .Include(b => b.GoodsReceiptItem)
                    .ThenInclude(gri => gri!.Medicine)
                .FirstOrDefaultAsync(b => b.BatchID == batchID);
        }

        public async Task<Dictionary<long, Batch>> GetBatchesByIdsAsync(IEnumerable<long> batchIds)
        {
            var ids = batchIds.ToList();
            if (ids.Count == 0)
                return new Dictionary<long, Batch>();

            var batches = await _context.Batch
                .Include(b => b.GoodsReceiptItem)
                    .ThenInclude(gri => gri!.GoodsReceipt)
                .Include(b => b.GoodsReceiptItem)
                    .ThenInclude(gri => gri!.Medicine)
                .Where(b => ids.Contains(b.BatchID))
                .ToListAsync();

            return batches.ToDictionary(b => b.BatchID);
        }

        public async Task<bool> IsCustomerExistsAsync(long customerId)
        {
            return await _context.Customer.AnyAsync(c => c.CustomerID == customerId);
        }

        public async Task<bool> IsMedicineExistsAsync(long medicineId)
        {
            return await _context.Medicine.AnyAsync(m => m.MedicineID == medicineId);
        }

        public async Task<List<Medicine>> GetMedicinesByIdsAsync(IEnumerable<long> medicineIds)
        {
            var ids = medicineIds.ToList();
            if (ids.Count == 0)
                return new List<Medicine>();

            return await _context.Medicine
                .Where(m => ids.Contains(m.MedicineID))
                .ToListAsync();
        }

        public async Task<bool> TryDecrementStockAsync(long batchId, decimal quantity)
        {
            return await _context.Batch
                .Where(b => b.BatchID == batchId && b.QuantityInStock >= quantity)
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.QuantityInStock, b => b.QuantityInStock - quantity)) > 0;
        }

        public async Task IncrementStockAsync(long batchId, decimal quantity)
        {
            await _context.Batch
                .Where(b => b.BatchID == batchId)
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.QuantityInStock, b => b.QuantityInStock + quantity));
        }

        public void RemoveInvoiceItems(IEnumerable<InvoiceItem> items)
        {
            _context.InvoiceItem.RemoveRange(items);
        }
    }
}
