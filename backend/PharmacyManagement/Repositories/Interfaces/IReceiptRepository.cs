using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface IReceiptRepository
    {
        Task<bool> IsCustomerExistsAsync(long customerId);
        Task AddAsync(Receipt entity);
        void Update(Receipt entity);
        void Delete(Receipt entity);
        Task<Receipt?> GetByIdAsync(long id);
        IQueryable<Receipt> GetQuery();
        Task<List<Receipt>> GetAllAsync(int skip, int take, long? customerId);
        Task<int> CountAsync(long? customerId);
        Task SaveChangesAsync();
    }
}
