using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly PharmacySystemDbContext _context;

        public WarehouseRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsBranchExistsAsync(long branchId)
        {
            return await _context.Branch.AnyAsync(x => x.BranchID == branchId);
        }

        public async Task<WareHouse?> GetByIdAsync(long id)
        {
            return await _context.WareHouse.FindAsync(id);
        }

        public async Task<List<WareHouse>> GetAllAsync(int skip, int take)
        {
            return await _context.WareHouse.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<List<WareHouse>> GetByBranchAsync(long branchId, int skip, int take)
        {
            return await _context.WareHouse
                .Where(w => w.BranchID == branchId)
                .Skip(skip).Take(take).ToListAsync();
        }

        public async Task<WareHouse?> GetByBranchIdAsync(long branchId)
        {
            return await _context.WareHouse
                .FirstOrDefaultAsync(w => w.BranchID == branchId);
        }

        public async Task<int> CountAsync()
        {
            return await _context.WareHouse.CountAsync();
        }

        public async Task<int> CountByBranchAsync(long branchId)
        {
            return await _context.WareHouse.CountAsync(w => w.BranchID == branchId);
        }

        public async Task AddAsync(WareHouse entity)
        {
            await _context.WareHouse.AddAsync(entity);
        }

        public void Update(WareHouse entity)
        {
            _context.WareHouse.Update(entity);
        }

        public void Delete(WareHouse entity)
        {
            _context.WareHouse.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Batch>> GetBatchesByWarehouseAsync(long warehouseId, int skip, int take)
        {
            return await _context.Batch
                .Include(b => b.GoodsReceiptItem)
                    .ThenInclude(gri => gri!.Medicine)
                .Where(b => b.WarehouseID == warehouseId)
                .OrderBy(b => b.ExpiryDate)
                .Skip(skip).Take(take)
                .ToListAsync();
        }

        public async Task<int> CountBatchesByWarehouseAsync(long warehouseId)
        {
            return await _context.Batch
                .Where(b => b.WarehouseID == warehouseId)
                .CountAsync();
        }
    }
}
