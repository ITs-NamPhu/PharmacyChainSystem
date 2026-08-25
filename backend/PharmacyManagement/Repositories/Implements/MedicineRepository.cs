using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class MedicineRepository : IMedicineRepository
    {
        private readonly PharmacySystemDbContext _context;

        public MedicineRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public IQueryable<Models.Medicine> GetQuery()
        {
            return _context.Medicine.AsNoTracking();
        }

        public async Task<bool> IsMedicineNameExistAsync(string medicineName)
        {
            return await _context.Medicine.AnyAsync(x => x.MedicineName == medicineName);
        }

        public async Task<bool> IsExistsAsync(long id)
        {
            return await _context.Medicine.AnyAsync(x => x.MedicineID == id);
        }

        public async Task<bool> IsCategoryExistsAsync(long categoryId)
        {
            return await _context.MedicineCategory.AnyAsync(x => x.MedicineCategoryID == categoryId);
        }

        public async Task<bool> IsManufacturerExistsAsync(long manufacturerId)
        {
            return await _context.ManuFacturer.AnyAsync(x => x.ManufacturerID == manufacturerId);
        }

        public async Task<bool> IsUnitExistsAsync(long unitId)
        {
            return await _context.Unit.AnyAsync(x => x.UnitID == unitId);
        }

        public async Task<Models.Medicine?> GetByIdAsync(long id)
        {
            return await _context.Medicine
                .Include(m => m.MedicineCategory)
                .Include(m => m.ManuFacturer)
                .Include(m => m.Unit)
                .FirstOrDefaultAsync(m => m.MedicineID == id);
        }

        public async Task<List<Models.Medicine>> GetAllAsync(int skip, int take)
        {
            return await _context.Medicine
                .Include(m => m.MedicineCategory)
                .Include(m => m.ManuFacturer)
                .Include(m => m.Unit)
                .Skip(skip).Take(take).ToListAsync();
        }

        public async Task<List<Models.Medicine>> GetAllAsync()
        {
            return await _context.Medicine
                .Include(m => m.MedicineCategory)
                .Include(m => m.ManuFacturer)
                .Include(m => m.Unit)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Medicine.CountAsync();
        }

        public async Task AddAsync(Models.Medicine entity)
        {
            await _context.Medicine.AddAsync(entity);
        }

        public void Update(Models.Medicine entity)
        {
            _context.Medicine.Update(entity);
        }

        public void Delete(Models.Medicine entity)
        {
            _context.Medicine.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
