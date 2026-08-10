using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface IAuthenticationRepository
    {
        Task<User> GetByUserNameAsync(string username);


        Task SaveChangesAsync();
    }
}
