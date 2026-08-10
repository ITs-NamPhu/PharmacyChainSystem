using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> IsUserNameExistAsync(string userName,long branchID);

        Task<User> GetUserByIDAsync(long userID, long branchID);

        Task<List<User>> GetAllUsersByBranchAsync(long branchID, int pass, int count);

        Task<List<User>> GetActiveUsersByBranchAsync(long branchID, int pass, int count);

        Task<int> CountActiveUsersByBranchAsync(long branchID);

        Task AddAsync(User user);

        void DeleteAsync(User user);

        void UpdateAsync(User user);

        Task SaveChangesAsync();

        Task<UserBranch?> GetUserBranchAsync(long userId, long branchId);

        Task<string?> GetRoleNameAtBranchAsync(long userId, long branchId);

        Task AddUserBranchAsync(UserBranch userBranch);

        void UpdateUserBranch(UserBranch userBranch);

        Task<User?> GetUserByIdAsync(long userId);

        Task<Role?> GetRoleByIdAsync(long roleId);

        Task<Branch?> GetBranchByIdAsync(long branchId);
    }
}
