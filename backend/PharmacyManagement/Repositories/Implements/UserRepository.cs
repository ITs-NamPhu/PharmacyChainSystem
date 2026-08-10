using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly PharmacySystemDbContext _context;

        public UserRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsUserNameExistAsync(string userName, long branchID)
        {
            return await _context.User
                .Include(x => x.UserBranch).ThenInclude(ub => ub.Role)
                .AnyAsync(x => x.UserName == userName &&
                            x.UserBranch.Any(ub => ub.BranchID == branchID));
        }

        public async Task<User> GetUserByIDAsync(long userID, long branchID)
        {
            return await _context.User.
                Include(x => x.UserBranch).ThenInclude(ub => ub.Role).
                FirstOrDefaultAsync(x => x.UserID == userID && 
                                    x.UserBranch.Any(ub => ub.BranchID == branchID));
        }

        public async Task<List<User>> GetActiveUsersByBranchAsync(long branchID, int pass, int count)
        {
            return await _context.User.Include(x => x.UserBranch).ThenInclude(ub => ub.Role).
                                        Where(x => x.IsActive == true && x.UserBranch.Any(ub => ub.BranchID == branchID))
                                        .Skip(pass)
                                        .Take(count).ToListAsync();
        }

        public async Task<List<User>> GetAllUsersByBranchAsync(long branchID, int pass, int count)
        {
            return await _context.User.Include(x => x.UserBranch).ThenInclude(ub => ub.Role)
                                        .Where(x => x.IsActive == true && x.UserBranch.Any(ub => ub.BranchID == branchID))
                                        .Skip(pass)
                                        .Take(count).ToListAsync();
        }

        public async Task<int> CountActiveUsersByBranchAsync(long branchID)
        {
            return await _context.User.Include(x => x.UserBranch)
                .Where(x => x.IsActive && x.UserBranch.Any(ub => ub.BranchID == branchID))
                .CountAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.User.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void DeleteAsync(User user)
        {
            _context.User.Remove(user);
        }

        public void UpdateAsync(User user)
        {
            _context.User.Update(user);
        }

        public async Task<UserBranch?> GetUserBranchAsync(long userId, long branchId)
        {
            return await _context.UserBranch
                .FirstOrDefaultAsync(ub => ub.UserID == userId && ub.BranchID == branchId);
        }

        public async Task<string?> GetRoleNameAtBranchAsync(long userId, long branchId)
        {
            return await _context.UserBranch
                .Where(ub => ub.UserID == userId && ub.BranchID == branchId)
                .Select(ub => ub.Role!.RoleName)
                .FirstOrDefaultAsync();
        }

        public async Task AddUserBranchAsync(UserBranch userBranch)
        {
            await _context.UserBranch.AddAsync(userBranch);
        }

        public void UpdateUserBranch(UserBranch userBranch)
        {
            _context.UserBranch.Update(userBranch);
        }

        public async Task<User?> GetUserByIdAsync(long userId)
        {
            return await _context.User
                .Include(x => x.UserBranch).ThenInclude(ub => ub.Role)
                .FirstOrDefaultAsync(x => x.UserID == userId);
        }

        public async Task<Role?> GetRoleByIdAsync(long roleId)
        {
            return await _context.Role.FindAsync(roleId);
        }

        public async Task<Branch?> GetBranchByIdAsync(long branchId)
        {
            return await _context.Branch.FindAsync(branchId);
        }

    }
}
