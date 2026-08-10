using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;

namespace PharmacyManagement.Services.Implements
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _repository;

        public PermissionService(IPermissionRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<string>> GetPermission(long userId)
        {
            return await _repository.GetPermissionsByUserIdAsync(userId);
        }
    }
}
