using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface IUnitRepository
    {
        Task<bool> IsUnitNameExistAsync(string unitName);
        Task<bool> IsExistsAsync(long id);
        Task<Unit?> GetByIdAsync(long id);
        Task<List<Unit>> GetByIdsAsync(IEnumerable<long> ids);
        Task<List<Unit>> GetAllAsync();
        Task<List<Unit>> GetAllAsync(int skip, int take);
        Task<int> CountAsync();
        Task AddAsync(Unit entity);
        void Update(Unit entity);
        void Delete(Unit entity);
        Task SaveChangesAsync();
    }
}
