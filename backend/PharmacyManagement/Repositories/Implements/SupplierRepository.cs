using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly PharmacySystemDbContext _context;

        public SupplierRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsSupplierNameExistAsync(string supplierName)
        {
            return await _context.Supplier.AnyAsync(x => x.SupplierName == supplierName);
        }

        public async Task<Supplier?> GetByIdAsync(long id)
        {
            return await _context.Supplier.FindAsync(id);
        }

        public async Task<List<Supplier>> GetAllAsync(int skip, int take)
        {
            return await _context.Supplier.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Supplier.CountAsync();
        }

        public async Task AddAsync(Supplier entity)
        {
            await _context.Supplier.AddAsync(entity);
        }

        public void Update(Supplier entity)
        {
            _context.Supplier.Update(entity);
        }

        public void Delete(Supplier entity)
        {
            _context.Supplier.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
