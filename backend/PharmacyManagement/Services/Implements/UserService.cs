using PharmacyManagement.DTOs.User;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Mappers;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.Validators.BusinessRule;
using System.Net;

namespace PharmacyManagement.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly UserBusinessValidator _businessValidator;

        public UserService(IUserRepository repository, UserBusinessValidator businessValidator)
        {
            _repository = repository;
            _businessValidator = businessValidator;
        }

        public async Task<UserResponse> CreateAsync(CreateUserRequest request)
        {
            var user = request.ToEntity();

            await _repository.AddAsync(user);
            await _repository.SaveChangesAsync();

            return user.ToResponse();
        }

        public async Task<UserResponse> UpdateAsync(long id, long branchId, UpdateUserRequest request)
        {
            User user = await _repository.GetUserByIDAsync(id, branchId);

            if (user == null)
            {
                throw new BusinessException(
                    "User not found.",
                    "USER004",
                    StatusCodes.Status404NotFound);
            }

            _businessValidator.ValidateUserBelongsToBranch(user, branchId);

            request.ApplyTo(user);

            _repository.UpdateAsync(user);
            await _repository.SaveChangesAsync();

            return user.ToResponse();
        }

        public async Task DeleteAsync(long userID, long branchID)
        {
            var user = await _repository.GetUserByIDAsync(userID, branchID);
            if (user == null)
            {
                throw new BusinessException(
                    "User not found.",
                    "USER404",
                    StatusCodes.Status404NotFound);
            }

            user.IsActive = false;

            await _repository.SaveChangesAsync();
        }

        public async Task<List<UserResponse>> GetAllUserbyBranchAsync_Service(long branchID, int page, int count)
        {
            int passRows = page * count;

            List<User> users = await _repository.GetAllUsersByBranchAsync(branchID, passRows, count);

            if (users == null)
                throw new NotImplementedException();

            return users.ToResponseList();
        }

        public async Task<UserResponse> GetUserByIDAsync_Service(long userID, long branchID)
        {
            User user = await _repository.GetUserByIDAsync(userID, branchID);
            if (user == null)
                throw new NotImplementedException();

            return user.ToResponse();
        }

        public async Task<UserListResponse> GetAllUserActiveAsync_Service(long branchID, int page, int count)
        {
            Console.WriteLine("brandid:{0}", branchID);
            Console.WriteLine("page:{0}", page);
            Console.WriteLine("limitpage:{0}", count);

            int cal_page = page - 1;
            int passRows = cal_page * count;
            List<User> users = await _repository.GetActiveUsersByBranchAsync(branchID, passRows, count);

            if (users == null)
                throw new NotImplementedException();

            int num_records = await _repository.CountActiveUsersByBranchAsync(branchID);
            float totalPage = (float)Math.Ceiling((double)num_records / count);

            return new UserListResponse
            {
                TotalPage = totalPage,
                NumRecords = num_records,
                Users = users.ToResponseList()
            };
        }

        public async Task<UserResponse> AssignRoleToUserAsync(
            long userId,
            UserAssignmentRequest request,
            long callerUserId,
            long callerBranchId)
        {
            var callerRoleName = await _repository.GetRoleNameAtBranchAsync(callerUserId, callerBranchId);
            if (string.IsNullOrEmpty(callerRoleName))
            {
                throw new BusinessException(
                    "Caller does not have a role at this branch.",
                    "USER_ASSIGN_001",
                    StatusCodes.Status403Forbidden);
            }

            var targetRole = await _repository.GetRoleByIdAsync(request.RoleId);
            if (targetRole == null)
            {
                throw new BusinessException(
                    "Target role not found.",
                    "USER_ASSIGN_002",
                    StatusCodes.Status404NotFound);
            }

            var targetBranch = await _repository.GetBranchByIdAsync(callerBranchId);
            if (targetBranch == null)
            {
                throw new BusinessException(
                    "Target branch not found.",
                    "USER_ASSIGN_003",
                    StatusCodes.Status404NotFound);
            }

            var user = await _repository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new BusinessException(
                    "User not found.",
                    "USER_ASSIGN_004",
                    StatusCodes.Status404NotFound);
            }

            var allowedRoles = new Dictionary<string, string[]>
            {
                { "admin", new[] { "admin", "manage_supplier", "manage_branch", "user_sale", "user_warehouse" } },
                { "manage_supplier", new[] { "manage_supplier", "manage_branch", "user_sale", "user_warehouse" } },
                { "manage_branch", new[] { "manage_branch", "user_sale", "user_warehouse" } },
            };

            if (!allowedRoles.ContainsKey(callerRoleName))
            {
                throw new BusinessException(
                    "You do not have permission to assign roles.",
                    "USER_ASSIGN_005",
                    StatusCodes.Status403Forbidden);
            }

            var callerAllowedRoles = allowedRoles[callerRoleName];
            if (!callerAllowedRoles.Contains(targetRole.RoleName))
            {
                throw new BusinessException(
                    $"You cannot assign the role '{targetRole.RoleName}'.",
                    "USER_ASSIGN_006",
                    StatusCodes.Status403Forbidden);
            }

            var existingAssignment = await _repository.GetUserBranchAsync(userId, callerBranchId);
            if (existingAssignment != null)
            {
                existingAssignment.RoleID = request.RoleId;
                _repository.UpdateUserBranch(existingAssignment);
            }
            else
            {
                await _repository.AddUserBranchAsync(new UserBranch
                {
                    UserID = userId,
                    BranchID = callerBranchId,
                    RoleID = request.RoleId,
                });
            }

            await _repository.SaveChangesAsync();

            var updatedUser = await _repository.GetUserByIdAsync(userId);
            return updatedUser!.ToResponse();
        }
    }
}
