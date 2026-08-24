using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class GoodsReceiptRepository : IGoodsReceiptRepository
    {
        private readonly PharmacySystemDbContext _context;
        private IDbContextTransaction? _transaction;

        public GoodsReceiptRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<GoodsReceipt?> GetByIdAsync(long id)
        {
            return await _context.GoodsReceipt
                .Include(gr => gr.Supplier)
                .Include(gr => gr.User)
                .Include(gr => gr.GoodsReceiptItem)
                    .ThenInclude(item => item.Medicine)
                .Include(gr => gr.GoodsReceiptItem)
                    .ThenInclude(item => item.Unit)
                .Include(gr => gr.GoodsReceiptItem)
                    .ThenInclude(item => item.Batch)
                .FirstOrDefaultAsync(gr => gr.GoodsReceiptID == id);
        }

        public async Task<List<GoodsReceipt>> GetAllAsync(int skip, int take, long branchId)
        {
            return await _context.GoodsReceipt
                .Include(gr => gr.Supplier)
                .Include(gr => gr.User)
                .Where(gr => gr.BranchID == branchId)
                .OrderByDescending(gr => gr.ReceiptDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountAsync(long branchId)
        {
            return await _context.GoodsReceipt
                .CountAsync(gr => gr.BranchID == branchId);
        }

        public async Task<long> GetNextReceiptNumberAsync()
        {
            // IgnoreQueryFilters: vẫn tính cả phiếu đã xóa mềm để không bị trùng số phiếu
            var maxNumber = await _context.GoodsReceipt
                .IgnoreQueryFilters()
                .MaxAsync(gr => (long?)gr.ReceiptNumber) ?? 0;
            return maxNumber + 1;
        }

        public async Task AddAsync(GoodsReceipt entity)
        {
            await _context.GoodsReceipt.AddAsync(entity);
        }

        public void Update(GoodsReceipt entity)
        {
            _context.GoodsReceipt.Update(entity);
        }

        public void Delete(GoodsReceipt entity)
        {
            _context.GoodsReceipt.Remove(entity);
        }

        public void RemoveGoodsReceiptItems(IEnumerable<GoodsReceiptItem> items)
        {
            _context.GoodsReceiptItem.RemoveRange(items);
        }

        public void RemoveBatches(IEnumerable<Batch> batches)
        {
            _context.Batch.RemoveRange(batches);
        }

        public async Task<GoodsReceiptItem?> GetItemByIdAsync(long itemId)
        {
            return await _context.GoodsReceiptItem
                .Include(item => item.Batch)
                .FirstOrDefaultAsync(item => item.GoodsReceiptItemID == itemId);
        }

        public async Task<Batch?> GetBatchByIdAsync(long batchId)
        {
            return await _context.Batch.FindAsync(batchId);
        }

        public async Task<bool> HasAnyReferenceForBatchesAsync(IEnumerable<long> batchIds)
        {
            var ids = batchIds.ToList();
            if (ids.Count == 0) return false;

            // chặn xóa khi batch của phiếu được tham chiếu bởi bất kỳ bảng nào
            return await _context.InvoiceItem.AnyAsync(ii => ids.Contains(ii.BatchID))
                || await _context.PurchaseReturnItem.AnyAsync(pri => ids.Contains(pri.BatchID))
                || await _context.SalesReturnItem.AnyAsync(sri => ids.Contains(sri.BatchID))
                || await _context.DestroyReceiptItem.AnyAsync(dri => ids.Contains(dri.BatchID))
                || await _context.StockAdjustmentItem.AnyAsync(sai => ids.Contains(sai.BatchID))
                || await _context.StockTakeItem.AnyAsync(sti => ids.Contains(sti.BatchID));
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

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
