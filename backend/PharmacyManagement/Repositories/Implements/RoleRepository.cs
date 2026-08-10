using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class RoleRepository : IRoleRepository
    {
        private readonly PharmacySystemDbContext _context;

        public RoleRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsRoleNameExistAsync(string roleName)
        {
            return await _context.Role.AnyAsync(x => x.RoleName == roleName);
        }

        public async Task<Role?> GetByIdAsync(long id)
        {
            return await _context.Role.FindAsync(id);
        }

        public async Task<List<Role>> GetAllAsync(int skip, int take)
        {
            return await _context.Role.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await _context.Role.ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Role.CountAsync();
        }

        public async Task AddAsync(Role entity)
        {
            await _context.Role.AddAsync(entity);
        }

        public void Update(Role entity)
        {
            _context.Role.Update(entity);
        }

        public void Delete(Role entity)
        {
            _context.Role.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
