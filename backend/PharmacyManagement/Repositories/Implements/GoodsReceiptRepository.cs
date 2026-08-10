using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class GoodsReceiptRepository : IGoodsReceiptRepository
    {
        private readonly PharmacySystemDbContext _context;

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
            var maxNumber = await _context.GoodsReceipt
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

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
