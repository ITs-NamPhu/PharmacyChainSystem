using PharmacyManagement.DTOs.User;
using PharmacyManagement.Models;
using PharmacyManagement.share;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> CreateAsync(CreateUserRequest request);

        Task<UserResponse> UpdateAsync(long id, long branchId, UpdateUserRequest request);

        Task DeleteAsync(long userID, long branchID);

        Task<UserListResponse> GetAllUserActiveAsync_Service(long branchID, int page, int count);

        Task<UserResponse> GetUserByIDAsync_Service(long userID, long branchID);

        Task<List<UserResponse>> GetAllUserbyBranchAsync_Service(long branchID, int page, int count);

        Task<UserResponse> AssignRoleToUserAsync(long userId, UserAssignmentRequest request, long callerUserId, long callerBranchId);
    }
}
