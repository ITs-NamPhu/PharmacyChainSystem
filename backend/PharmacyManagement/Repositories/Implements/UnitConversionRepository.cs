using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class UnitConversionRepository : IUnitConversionRepository
    {
        private readonly PharmacySystemDbContext _context;

        public UnitConversionRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<UnitConversion?> GetByIdAsync(long id)
        {
            return await _context.UnitConversion.FindAsync(id);
        }

        public async Task<List<UnitConversion>> GetAllAsync()
        {
            return await _context.UnitConversion.Include(uc => uc.Medicine).ToListAsync();
        }

        public async Task<List<UnitConversion>> GetAllAsync(int skip, int take)
        {
            return await _context.UnitConversion.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<List<UnitConversion>> getUniconver()
        {
            return await _context.UnitConversion
                .Include(uc => uc.Medicine)
                .ToListAsync();
        }
        public async Task<int> CountAsync()
        {
            return await _context.UnitConversion.CountAsync();
        }

        public async Task AddAsync(UnitConversion entity)
        {
            await _context.UnitConversion.AddAsync(entity);
        }

        public void Update(UnitConversion entity)
        {
            _context.UnitConversion.Update(entity);
        }

        public void Delete(UnitConversion entity)
        {
            _context.UnitConversion.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<UnitConversion?> GetByMedicineAndUnitAsync(long medicineId, long unitId)
        {
            return await _context.UnitConversion
                .FirstOrDefaultAsync(uc => uc.MedicineID == medicineId && uc.UnitID == unitId);
        }

        public async Task<List<UnitConversion>> GetByMedicineIdsAsync(IEnumerable<long> medicineIds)
        {
            var ids = medicineIds.ToList();
            if (ids.Count == 0)
                return new List<UnitConversion>();

            return await _context.UnitConversion
                .Where(uc => ids.Contains(uc.MedicineID))
                .ToListAsync();
        }
    }
}
