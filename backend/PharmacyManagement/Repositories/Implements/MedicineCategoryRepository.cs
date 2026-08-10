using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class MedicineCategoryRepository : IMedicineCategoryRepository
    {
        private readonly PharmacySystemDbContext _context;

        public MedicineCategoryRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsCategoryNameExistAsync(string categoryName)
        {
            return await _context.MedicineCategory.AnyAsync(x => x.CategoryName == categoryName);
        }

        public async Task<MedicineCategory?> GetByIdAsync(long id)
        {
            return await _context.MedicineCategory.FindAsync(id);
        }

        public async Task<List<MedicineCategory>> GetAllAsync()
        {
            return await _context.MedicineCategory.ToListAsync();
        }

        public async Task<List<MedicineCategory>> GetAllAsync(int skip, int take)
        {
            return await _context.MedicineCategory.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.MedicineCategory.CountAsync();
        }

        public async Task AddAsync(MedicineCategory entity)
        {
            await _context.MedicineCategory.AddAsync(entity);
        }

        public void Update(MedicineCategory entity)
        {
            _context.MedicineCategory.Update(entity);
        }

        public void Delete(MedicineCategory entity)
        {
            _context.MedicineCategory.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
