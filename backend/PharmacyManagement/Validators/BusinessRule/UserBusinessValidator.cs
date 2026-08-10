using PharmacyManagement.Exceptions;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;
using System.Net;

namespace PharmacyManagement.Validators.BusinessRule
{
    public class UserBusinessValidator
    {
        private readonly IUserRepository _repository;

        public UserBusinessValidator(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task ValidateUserNameNotExistsAsync(string userName, long branchId)
        {
            if (await _repository.IsUserNameExistAsync(userName, branchId))
            {
                throw new BusinessException(
                    "Username already exists.",
                    "USER001",
                    StatusCodes.Status409Conflict);
            }
        }

        public void ValidateUserBelongsToBranch(User user, long branchId)
        {
            bool hasBranch = user.UserBranch.Any(x => x.BranchID == branchId);

            if (!hasBranch)
            {
                throw new BusinessException(
                    "User does not belong to this branch.",
                    "USER005",
                    StatusCodes.Status403Forbidden);
            }
        }
    }
}
