using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class CustomerTypeRepository : ICustomerTypeRepository
    {
        private readonly PharmacySystemDbContext _context;

        public CustomerTypeRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsTypeNameExistAsync(string typeName)
        {
            return await _context.CustomerType.AnyAsync(x => x.TypeName == typeName);
        }

        public async Task<CustomerType?> GetByIdAsync(long id)
        {
            return await _context.CustomerType.FindAsync(id);
        }

        public async Task<List<CustomerType>> GetAllAsync()
        {
            return await _context.CustomerType.ToListAsync();
        }

        public async Task<List<CustomerType>> GetAllAsync(int skip, int take)
        {
            return await _context.CustomerType.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.CustomerType.CountAsync();
        }

        public async Task AddAsync(CustomerType entity)
        {
            await _context.CustomerType.AddAsync(entity);
        }

        public void Update(CustomerType entity)
        {
            _context.CustomerType.Update(entity);
        }

        public void Delete(CustomerType entity)
        {
            _context.CustomerType.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
