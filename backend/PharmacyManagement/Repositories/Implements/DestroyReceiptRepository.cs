using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PharmacyManagement.DTOs.DestroyReceipt;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class DestroyReceiptRepository : IDestroyReceiptRepository
    {
        private readonly PharmacySystemDbContext _context;
        private IDbContextTransaction? _transaction;

        public DestroyReceiptRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<DestroyReceipt?> GetByIdAsync(long id)
        {
            return await _context.DestroyReceipt
                .Include(dr => dr.WareHouse)
                .Include(dr => dr.User)
                .Include(dr => dr.StockTake)
                .Include(dr => dr.DestroyReceiptItem)
                    .ThenInclude(item => item.Batch)
                        .ThenInclude(batch => batch!.GoodsReceiptItem)
                            .ThenInclude(gri => gri!.Medicine)
                .FirstOrDefaultAsync(dr => dr.DestroyReceiptID == id);
        }

        public async Task<List<DestroyReceiptResponse>> GetAllAsync(int skip, int take, long warehouseId)
        {
            var query = _context.DestroyReceipt.AsQueryable();

            if (warehouseId > 0)
                query = query.Where(dr => dr.WarehouseID == warehouseId);

            return await query
                .OrderByDescending(dr => dr.CreatedAt)
                .Skip(skip)
                .Take(take)
                .Select(dr => new DestroyReceiptResponse
                {
                    DestroyReceiptID = dr.DestroyReceiptID,
                    WarehouseID = dr.WarehouseID,
                    WarehouseName = dr.WareHouse != null ? dr.WareHouse.WarehouseName : string.Empty,
                    StockTakeID = dr.StockTakeID,
                    UserID = dr.UserID,
                    UserName = dr.User != null ? dr.User.FullName : string.Empty,
                    Note = dr.Note,
                    CreatedAt = dr.CreatedAt,
                    Status = dr.Status.ToString(),
                    ApprovedBy = dr.ApprovedBy,
                    ApprovedAt = dr.ApprovedAt,
                    IsFromStockTake = dr.StockTakeID.HasValue,
                    ItemCount = dr.DestroyReceiptItem != null ? dr.DestroyReceiptItem.Count() : 0
                })
                .ToListAsync();
        }

        public async Task<int> CountAsync(long warehouseId)
        {
            if (warehouseId > 0)
                return await _context.DestroyReceipt.CountAsync(dr => dr.WarehouseID == warehouseId);

            return await _context.DestroyReceipt.CountAsync();
        }

        public async Task AddAsync(DestroyReceipt entity)
        {
            await _context.DestroyReceipt.AddAsync(entity);
        }

        public void Delete(DestroyReceipt entity)
        {
            _context.DestroyReceipt.Remove(entity);
        }

        public async Task<WareHouse?> GetWarehouseByIdAsync(long warehouseId)
        {
            return await _context.WareHouse.FindAsync(warehouseId);
        }

        public async Task<bool> HasDestroyReceiptAsync(long stockTakeId)
        {
            return await _context.DestroyReceipt.AnyAsync(dr => dr.StockTakeID == stockTakeId);
        }

        public async Task<StockTake?> GetStockTakeByIdAsync(long stockTakeId)
        {
            return await _context.StockTake
                .Include(st => st.StockTakeItem)
                .FirstOrDefaultAsync(st => st.StockTakeID == stockTakeId);
        }

        public async Task<Dictionary<long, Batch>> GetBatchesByIdsAsync(IEnumerable<long> batchIds)
        {
            var ids = batchIds.ToList();
            if (ids.Count == 0)
                return new Dictionary<long, Batch>();

            var batches = await _context.Batch
                .Include(b => b.GoodsReceiptItem)
                    .ThenInclude(gri => gri!.Medicine)
                .Where(b => ids.Contains(b.BatchID))
                .ToListAsync();

            return batches.ToDictionary(b => b.BatchID);
        }

        public async Task<Dictionary<long, StockTakeItem>> GetStockTakeItemsByIdsAsync(IEnumerable<long> stockTakeItemIds)
        {
            var ids = stockTakeItemIds.Distinct().ToList();
            if (ids.Count == 0)
                return new Dictionary<long, StockTakeItem>();

            var items = await _context.StockTakeItem
                .Where(i => ids.Contains(i.StockTakeItemID))
                .ToListAsync();

            return items.ToDictionary(i => i.StockTakeItemID);
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
    }
}
