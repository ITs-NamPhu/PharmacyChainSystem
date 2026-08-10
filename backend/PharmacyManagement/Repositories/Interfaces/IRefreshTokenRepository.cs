using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
            Task AddAsync(RefreshToken refreshToken);


    Task RevokeByHashAsync(string hash);

    Task<RefreshToken?> GetByHashAsync(string hash);

            Task SaveChangesAsync();
    }
}
