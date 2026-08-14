using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class StockAdjustmentRepository : IStockAdjustmentRepository
    {
        private readonly PharmacySystemDbContext _context;
        private IDbContextTransaction? _transaction;

        public StockAdjustmentRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<StockAdjustment?> GetByIdAsync(long id)
        {
            return await _context.StockAdjustment
                .Include(sa => sa.WareHouse)
                .Include(sa => sa.User)
                .Include(sa => sa.StockTake)
                .Include(sa => sa.StockAdjustmentItem)
                    .ThenInclude(item => item.Batch)
                        .ThenInclude(batch => batch!.GoodsReceiptItem)
                            .ThenInclude(gri => gri!.Medicine)
                .FirstOrDefaultAsync(sa => sa.StockAdjustmentID == id);
        }

        public async Task<List<StockAdjustment>> GetAllAsync(int skip, int take, long warehouseId)
        {
            var query = _context.StockAdjustment
                .Include(sa => sa.WareHouse)
                .Include(sa => sa.User)
                .Include(sa => sa.StockAdjustmentItem)
                .AsQueryable();

            if (warehouseId > 0)
                query = query.Where(sa => sa.WarehouseID == warehouseId);

            return await query
                .OrderByDescending(sa => sa.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountAsync(long warehouseId)
        {
            if (warehouseId > 0)
                return await _context.StockAdjustment.CountAsync(sa => sa.WarehouseID == warehouseId);

            return await _context.StockAdjustment.CountAsync();
        }

        public async Task AddAsync(StockAdjustment entity)
        {
            await _context.StockAdjustment.AddAsync(entity);
        }

        public async Task<bool> HasAdjustmentAsync(long stockTakeId)
        {
            return await _context.StockAdjustment.AnyAsync(sa => sa.StockTakeID == stockTakeId);
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
