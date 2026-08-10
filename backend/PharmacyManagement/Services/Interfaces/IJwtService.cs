using PharmacyManagement.Models;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);

    }
}
