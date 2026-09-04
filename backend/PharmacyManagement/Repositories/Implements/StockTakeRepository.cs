using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class StockTakeRepository : IStockTakeRepository
    {
        private readonly PharmacySystemDbContext _context;
        private IDbContextTransaction? _transaction;

        public StockTakeRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<StockTake?> GetByIdAsync(long id)
        {
            return await _context.StockTake
                .Include(st => st.WareHouse)
                .Include(st => st.User)
                .Include(st => st.StockTakeItem)
                    .ThenInclude(item => item.Batch)
                        .ThenInclude(batch => batch!.GoodsReceiptItem)
                            .ThenInclude(gri => gri!.Medicine)
                .Include(st => st.StockAdjustment)
                .Include(st => st.DestroyReceipt)
                .FirstOrDefaultAsync(st => st.StockTakeID == id);
        }

        public async Task<List<StockTake>> GetAllAsync(int skip, int take, long warehouseId)
        {
            var query = _context.StockTake
                .Include(st => st.WareHouse)
                .Include(st => st.User)
                .Include(st => st.StockTakeItem)
                .AsQueryable();

            if (warehouseId > 0)
                query = query.Where(st => st.WarehouseID == warehouseId);

            return await query
                .OrderByDescending(st => st.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountAsync(long warehouseId)
        {
            if (warehouseId > 0)
                return await _context.StockTake.CountAsync(st => st.WarehouseID == warehouseId);

            return await _context.StockTake.CountAsync();
        }

        public async Task AddAsync(StockTake entity)
        {
            await _context.StockTake.AddAsync(entity);
        }

        public void Update(StockTake entity)
        {
            _context.StockTake.Update(entity);
        }

        public void Delete(StockTake entity)
        {
            _context.StockTake.Remove(entity);
        }

        public async Task<WareHouse?> GetWarehouseByIdAsync(long warehouseId)
        {
            return await _context.WareHouse.FindAsync(warehouseId);
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

        public async Task AddDestroyReceiptAsync(DestroyReceipt entity)
        {
            await _context.DestroyReceipt.AddAsync(entity);
        }

        public async Task AddStockAdjustmentAsync(StockAdjustment entity)
        {
            await _context.StockAdjustment.AddAsync(entity);
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
