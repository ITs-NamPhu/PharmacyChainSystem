using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly PharmacySystemDbContext _context;

        public CustomerRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsCustomerTypeExistsAsync(long customerTypeId)
        {
            return await _context.CustomerType.AnyAsync(x => x.CustomerTypeID == customerTypeId);
        }

        public async Task<Customer?> GetByIdAsync(long id)
        {
            return await _context.Customer
                .Include(c => c.CustomerType)
                .FirstOrDefaultAsync(c => c.CustomerID == id);
        }

        public async Task<List<Customer>> GetAllAsync(int skip, int take)
        {
            return await _context.Customer
                .Include(c => c.CustomerType)
                .Skip(skip).Take(take).ToListAsync();
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _context.Customer
                .Include(c => c.CustomerType)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Customer.CountAsync();
        }

        public async Task AddAsync(Customer entity)
        {
            await _context.Customer.AddAsync(entity);
        }

        public void Update(Customer entity)
        {
            _context.Customer.Update(entity);
        }

        public void Delete(Customer entity)
        {
            _context.Customer.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
