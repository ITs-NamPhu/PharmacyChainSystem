using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using PharmacyManagement.DTOs.Auth;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Implements;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
using System.Security.Claims;

namespace PharmacyManagement.Services.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IAuthenticationRepository _repository;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(IAuthenticationRepository repository, IJwtService jwtService, IRefreshTokenService refreshTokenService, IRefreshTokenRepository refreshTokenRepository)
        {
            _repository = repository;
            _refreshTokenRepository = refreshTokenRepository;

            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
        }


        [AllowAnonymous]
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _repository.GetByUserNameAsync(request.UserName);

            if (user == null)
            {
                throw new BusinessException(
                    "Username is incorrect.",
                    "AUTH001",
                    StatusCodes.Status401Unauthorized);
            }

            // nếu tài khoảng có thời gian khóa lớn hơn thời gian hiện tại thì throw lỗi
            // do đăng nhập sai quá nhiều lần
            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
                throw new BusinessException(
                                    "Account is locked.",
                                    "AUTH002",
                                    StatusCodes.Status423Locked);


            bool verify = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            // nếu verify false thì tăng số lần đăng nhập thất bại 
            // và lưu thời gian đăng nhập thất bại cuối cùng
            //      nếu số lần đăng nhập thất bại >= 5 thì khóa tài khoản trong 15 phút
            // => throw lỗi
            if (!verify)
            {
                user.FailedLoginCount++;
                user.LastFailedLogin = DateTime.UtcNow;

                if (user.FailedLoginCount >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                    await _repository.SaveChangesAsync();
                }

                await _repository.SaveChangesAsync();

                throw new BusinessException(
                    "Username or password is incorrect.",
                    "AUTH003",
                    StatusCodes.Status401Unauthorized);
            }

            string token = _jwtService.GenerateToken(user);

            string generate_refreshToken = _refreshTokenService.GenerateRefreshToken();

            string hash = HashHelper.ComputeSha256(generate_refreshToken);


            var refreshtoken_db = new RefreshToken
            {
                UserID = user.UserID,
                Token = hash,
                CreatedAt = DateTime.Now,
                ExpireAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
            };


            user.FailedLoginCount = 0;
            user.LockoutEnd = null;

            await _refreshTokenRepository.AddAsync(refreshtoken_db);

            await _repository.SaveChangesAsync();

            await _refreshTokenRepository.SaveChangesAsync();

            var roleName = user.UserBranch?.FirstOrDefault()?.Role?.RoleName ?? "";

            return new LoginResponse
            {
                AccessToken = token,

                RefreshToken = generate_refreshToken,

                User = new UserInfoResponse
                {
                    UserId = user.UserID,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = roleName
                },

                Branches = user.UserBranch?.Select(ub => new LoginBranchResponse
                {
                    BranchId = ub.BranchID,
                    BranchName = ub.Branch?.BranchName ?? "",
                    IsDefault = ub.IsDefault
                }).ToList() ?? new List<LoginBranchResponse>()
            };
        }

        public async Task<Boolean> LogoutAsync(LogoutRequest request)
        {
            string hash = HashHelper.ComputeSha256(request.refreshToken);

            await _refreshTokenRepository.RevokeByHashAsync(hash);

            return true;
        }

        [AllowAnonymous]
        public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            string hash = HashHelper.ComputeSha256(request.RefreshToken);

            var refresh_token = await _refreshTokenRepository.GetByHashAsync(hash);

            if (refresh_token == null)
            {
                throw new BusinessException(
                    "Refresh token is invalid.",
                    "AUTH003",
                    StatusCodes.Status401Unauthorized);
            }

            // nếu refresh token hết hạn
            if (refresh_token.ExpireAt < DateTime.UtcNow)
            {
                throw new BusinessException(
                    "Refresh token has expired.",
                    "AUTH004",
                    StatusCodes.Status401Unauthorized);
            }

            // nếu refresh token bị thu hồi
            if (refresh_token.IsRevoked == true)
            {
                throw new BusinessException(
                    "Refresh token is revoked.",
                    "AUTH004",
                    StatusCodes.Status401Unauthorized);
            }



            // tạo access token mới
            string newAccess = _jwtService.GenerateToken(refresh_token.User);

            // tạo refresh token mới
            string newRefresh = _refreshTokenService.GenerateRefreshToken();

            // mã hóa refresh token
            string refreshToken_hash = HashHelper.ComputeSha256(newRefresh);



            var refreshtoken_db = new RefreshToken
            {
                UserID = refresh_token.UserID,
                Token = refreshToken_hash,
                CreatedAt = DateTime.Now,
                ExpireAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
            };

            // thu hồi refreshtoken cũ
            await _refreshTokenRepository.RevokeByHashAsync(hash);

            await _refreshTokenRepository.SaveChangesAsync();

            await _refreshTokenRepository.AddAsync(refreshtoken_db);

            await _refreshTokenRepository.SaveChangesAsync();

            var refreshRoleName = refresh_token.User.UserBranch?.FirstOrDefault()?.Role?.RoleName ?? "";

            return new LoginResponse
            {
                AccessToken = newAccess,

                RefreshToken = newRefresh,

                User = new UserInfoResponse
                {
                    UserId = refresh_token.User.UserID,
                    UserName = refresh_token.User.UserName,
                    FullName = refresh_token.User.FullName,
                    Email = refresh_token.User.Email,
                    Phone = refresh_token.User.Phone,
                    Role = refreshRoleName
                },

                Branches = refresh_token.User.UserBranch?.Select(ub => new LoginBranchResponse
                {
                    BranchId = ub.BranchID,
                    BranchName = ub.Branch?.BranchName ?? "",
                    IsDefault = ub.IsDefault
                }).ToList() ?? new List<LoginBranchResponse>()
            };
        }
    }
}
