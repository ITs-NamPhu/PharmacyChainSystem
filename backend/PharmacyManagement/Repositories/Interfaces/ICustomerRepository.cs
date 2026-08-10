using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<bool> IsCustomerTypeExistsAsync(long customerTypeId);
        Task<Customer?> GetByIdAsync(long id);
        Task<List<Customer>> GetAllAsync(int skip, int take);
        Task<List<Customer>> GetAllAsync();
        Task<int> CountAsync();
        Task AddAsync(Customer entity);
        void Update(Customer entity);
        void Delete(Customer entity);
        Task SaveChangesAsync();
    }
}
