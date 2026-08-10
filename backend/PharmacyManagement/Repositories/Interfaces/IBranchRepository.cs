using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface IBranchRepository
    {
        Task<bool> IsPriceListExistsAsync(long priceListId);
        Task<Branch?> GetByIdAsync(long id);
        Task<List<Branch>> GetAllAsync();
        Task<List<Branch>> GetAllAsync(int skip, int take);
        Task<int> CountAsync();
        Task AddAsync(Branch entity);
        void Update(Branch entity);
        void Delete(Branch entity);
        Task SaveChangesAsync();
    }
}
