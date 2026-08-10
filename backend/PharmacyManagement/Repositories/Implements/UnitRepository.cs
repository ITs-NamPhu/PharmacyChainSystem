using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class UnitRepository : IUnitRepository
    {
        private readonly PharmacySystemDbContext _context;

        public UnitRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsUnitNameExistAsync(string unitName)
        {
            return await _context.Unit.AnyAsync(x => x.UnitName == unitName);
        }

        public async Task<bool> IsExistsAsync(long id)
        {
            return await _context.Unit.AnyAsync(x => x.UnitID == id);
        }

        public async Task<Unit?> GetByIdAsync(long id)
        {
            return await _context.Unit.FindAsync(id);
        }

        public async Task<List<Unit>> GetByIdsAsync(IEnumerable<long> ids)
        {
            var idList = ids.ToList();
            if (idList.Count == 0)
                return new List<Unit>();

            return await _context.Unit
                .Where(u => idList.Contains(u.UnitID))
                .ToListAsync();
        }

        public async Task<List<Unit>> GetAllAsync()
        {
            return await _context.Unit.ToListAsync();
        }

        public async Task<List<Unit>> GetAllAsync(int skip, int take)
        {
            return await _context.Unit.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Unit.CountAsync();
        }

        public async Task AddAsync(Unit entity)
        {
            await _context.Unit.AddAsync(entity);
        }

        public void Update(Unit entity)
        {
            _context.Unit.Update(entity);
        }

        public void Delete(Unit entity)
        {
            _context.Unit.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
