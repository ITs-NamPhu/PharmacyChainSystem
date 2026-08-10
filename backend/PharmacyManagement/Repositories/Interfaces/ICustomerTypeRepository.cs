using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface ICustomerTypeRepository
    {
        Task<bool> IsTypeNameExistAsync(string typeName);
        Task<CustomerType?> GetByIdAsync(long id);
        Task<List<CustomerType>> GetAllAsync();
        Task<List<CustomerType>> GetAllAsync(int skip, int take);
        Task<int> CountAsync();
        Task AddAsync(CustomerType entity);
        void Update(CustomerType entity);
        void Delete(CustomerType entity);
        Task SaveChangesAsync();
    }
}
