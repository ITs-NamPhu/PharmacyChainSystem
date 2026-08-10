using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly PharmacySystemDbContext _context;

        public PermissionRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetPermissionsByUserIdAsync(long userId)
        {
            return await _context.UserBranch
                .Where(ub => ub.UserID == userId)
                .SelectMany(ub => ub.Role!.RolePermission!)
                .Select(rp => rp.Permission!.Name)
                .Distinct()
                .ToListAsync();
        }
    }
}
