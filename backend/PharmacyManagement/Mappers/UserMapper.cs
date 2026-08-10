using PharmacyManagement.DTOs.User;
using PharmacyManagement.Models;
using BC = BCrypt.Net.BCrypt;

namespace PharmacyManagement.Mappers
{
    public static class UserMapper
    {
        public static User ToEntity(this CreateUserRequest request)
        {
            return new User
            {
                UserName = request.UserName,
                PasswordHash = BC.HashPassword(request.Password),
                FullName = request.FullName,
                Phone = request.Phone,
                Address = request.Address,
                Email = request.Email,
                IsActive = true,
                FailedLoginCount = 0,
            };
        }

        public static void ApplyTo(this UpdateUserRequest request, User user)
        {
            user.FullName = request.FullName;
            user.Phone = request.Phone;
            user.Address = request.Address;
            user.Email = request.Email;
            user.IsActive = request.IsActive;
        }

        public static UserResponse ToResponse(this User user)
        {
            var userBranch = user.UserBranch?.FirstOrDefault();

            return new UserResponse
            {
                UserID = user.UserID,
                FullName = user.FullName,
                Phone = user.Phone,
                Address = user.Address,
                Email = user.Email,
                IsActive = user.IsActive,
                RoleID = userBranch?.RoleID ?? 0,
                RoleName = userBranch?.Role?.RoleName,
                BranchID = userBranch?.BranchID ?? 0,
            };
        }

        public static List<UserResponse> ToResponseList(this List<User> users)
        {
            return users.Select(user => user.ToResponse()).ToList();
        }
    }
}
