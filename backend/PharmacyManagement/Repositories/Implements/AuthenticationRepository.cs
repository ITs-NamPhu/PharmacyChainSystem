using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly PharmacySystemDbContext _context;

        public AuthenticationRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByUserNameAsync(string username)
        {
            return await _context.User
                    .Include(x => x.UserBranch).ThenInclude(ub => ub.Role)
                    .Include(x => x.UserBranch).ThenInclude(ub => ub.Branch)
                    .FirstOrDefaultAsync(x =>
                        x.UserName == username &&
                        x.IsActive == true);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
