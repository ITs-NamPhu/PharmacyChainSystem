using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class ManufacturerRepository : IManufacturerRepository
    {
        private readonly PharmacySystemDbContext _context;

        public ManufacturerRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsManufacturerNameExistAsync(string manufacturerName)
        {
            return await _context.ManuFacturer.AnyAsync(x => x.ManufacturerName == manufacturerName);
        }

        public async Task<ManuFacturer?> GetByIdAsync(long id)
        {
            return await _context.ManuFacturer.FindAsync(id);
        }

        public async Task<List<ManuFacturer>> GetAllAsync()
        {
            return await _context.ManuFacturer.ToListAsync();
        }

        public async Task<List<ManuFacturer>> GetAllAsync(int skip, int take)
        {
            return await _context.ManuFacturer.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.ManuFacturer.CountAsync();
        }

        public async Task AddAsync(ManuFacturer entity)
        {
            await _context.ManuFacturer.AddAsync(entity);
        }

        public void Update(ManuFacturer entity)
        {
            _context.ManuFacturer.Update(entity);
        }

        public void Delete(ManuFacturer entity)
        {
            _context.ManuFacturer.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
