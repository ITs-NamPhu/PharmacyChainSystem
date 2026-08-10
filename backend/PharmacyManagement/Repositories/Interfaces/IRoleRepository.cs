using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<bool> IsRoleNameExistAsync(string roleName);
        Task<Role?> GetByIdAsync(long id);
        Task<List<Role>> GetAllAsync(int skip, int take);
        Task<List<Role>> GetAllAsync();
        Task<int> CountAsync();
        Task AddAsync(Role entity);
        void Update(Role entity);
        void Delete(Role entity);
        Task SaveChangesAsync();
    }
}
