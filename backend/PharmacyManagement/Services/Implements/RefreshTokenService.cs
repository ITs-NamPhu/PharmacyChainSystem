using PharmacyManagement.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace PharmacyManagement.Services.Implements
{
    public class RefreshTokenService : IRefreshTokenService
    {
        public long UserID { get; internal set; }

        public string GenerateRefreshToken()
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(randomBytes);
        }
    }

    public static class HashHelper
    {
        public static string ComputeSha256(string input)
        {
            using var sha = SHA256.Create();

            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToHexString(bytes);
        }
    }
}
