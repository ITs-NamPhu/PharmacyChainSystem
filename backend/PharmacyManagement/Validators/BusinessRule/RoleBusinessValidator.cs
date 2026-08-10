using PharmacyManagement.Exceptions;
using PharmacyManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Validators.BusinessRule
{
    public class RoleBusinessValidator
    {
        private readonly IRoleRepository _repository;

        public RoleBusinessValidator(IRoleRepository repository)
        {
            _repository = repository;
        }

        public async Task ValidateRoleNameNotExistsAsync(string roleName)
        {
            if (await _repository.IsRoleNameExistAsync(roleName))
            {
                throw new BusinessException(
                    "Role name already exists.",
                    "ROLE001",
                    StatusCodes.Status409Conflict);
            }
        }
    }
}
