using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class ReceiptRepository : IReceiptRepository
    {
        private readonly PharmacySystemDbContext _context;

        public ReceiptRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsCustomerExistsAsync(long customerId)
        {
            return await _context.Customer.AnyAsync(c => c.CustomerID == customerId);
        }

        public async Task AddAsync(Receipt entity)
        {
            await _context.Receipt.AddAsync(entity);
        }

        public void Update(Receipt entity)
        {
            _context.Receipt.Update(entity);
        }

        public void Delete(Receipt entity)
        {
            _context.Receipt.Remove(entity);
        }

        public async Task<Receipt?> GetByIdAsync(long id)
        {
            return await _context.Receipt
                .Include(r => r.Customer)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReceiptID == id);
        }

        public IQueryable<Receipt> GetQuery()
        {
            return _context.Receipt
                .Include(r => r.Customer)
                .Include(r => r.User)
                .AsNoTracking();
        }

        public async Task<List<Receipt>> GetAllAsync(int skip, int take, long? customerId)
        {
            IQueryable<Receipt> query = _context.Receipt
                .Include(r => r.Customer)
                .Include(r => r.User);

            if (customerId.HasValue)
                query = query.Where(r => r.CustomerID == customerId.Value);

            return await query
                .OrderByDescending(r => r.CreatedDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountAsync(long? customerId)
        {
            var query = _context.Receipt.AsQueryable();

            if (customerId.HasValue)
                query = query.Where(r => r.CustomerID == customerId.Value);

            return await query.CountAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
