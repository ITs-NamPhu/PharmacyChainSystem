using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class BranchRepository : IBranchRepository
    {
        private readonly PharmacySystemDbContext _context;

        public BranchRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsPriceListExistsAsync(long priceListId)
        {
            return await _context.PriceList.AnyAsync(x => x.PriceListID == priceListId);
        }

        public async Task<Branch?> GetByIdAsync(long id)
        {
            return await _context.Branch.FindAsync(id);
        }

        public async Task<List<Branch>> GetAllAsync()
        {
            return await _context.Branch.ToListAsync();
        }

        public async Task<List<Branch>> GetAllAsync(int skip, int take)
        {
            return await _context.Branch.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Branch.CountAsync();
        }

        public async Task AddAsync(Branch entity)
        {
            await _context.Branch.AddAsync(entity);
        }

        public void Update(Branch entity)
        {
            _context.Branch.Update(entity);
        }

        public void Delete(Branch entity)
        {
            _context.Branch.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
