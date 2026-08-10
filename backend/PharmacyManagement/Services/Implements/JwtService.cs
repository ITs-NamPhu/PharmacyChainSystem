using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PharmacyManagement.DTOs.Auth;
using PharmacyManagement.Models;
using PharmacyManagement.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PharmacyManagement.Services.Implements
{
    public class JwtService : IJwtService
    {
        private readonly JwtSetting _jwtSettings;

        public JwtService(IOptions<JwtSetting> options)
        {
            _jwtSettings = options.Value;
        }

        public string GenerateToken(User user)
        {
            var userBranch = user.UserBranch?.FirstOrDefault();
            var roleName = userBranch?.Role?.RoleName ?? "";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,
                    user.UserID.ToString()),

                new Claim(ClaimTypes.Name,
                    user.UserName),

                new Claim(ClaimTypes.Role,
                    roleName),
            };
            var key = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(

                issuer: _jwtSettings.Issuer,

                audience: _jwtSettings.Audience,

                claims: claims,

                expires:
                    DateTime.UtcNow.AddMinutes(
                        _jwtSettings.ExpireMinutes),

                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
