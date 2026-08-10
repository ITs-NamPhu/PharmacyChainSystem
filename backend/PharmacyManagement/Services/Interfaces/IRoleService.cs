using PharmacyManagement.DTOs.Role;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IRoleService
    {
        Task<RoleResponse> CreateAsync(CreateRoleRequest request);
        Task<RoleResponse> UpdateAsync(long id, UpdateRoleRequest request);
        Task DeleteAsync(long id);
        Task<RoleResponse?> GetByIdAsync(long id);
        Task<RoleListResponse> GetAllAsync(int page, int count);
        Task<List<RoleResponse>> GetAllRoleAsync();
    }
}
