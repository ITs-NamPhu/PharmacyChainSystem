using PharmacyManagement.DTOs.Auth;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<Boolean> LogoutAsync(LogoutRequest request);

    }
}
