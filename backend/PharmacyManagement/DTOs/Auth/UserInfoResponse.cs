namespace PharmacyManagement.DTOs.Auth
{
    public class UserInfoResponse
    {
        public long UserId { get; set; }

        public string UserName { get; set; } = "";

        public string FullName { get; set; } = "";

        public string Email { get; set; } = "";

        public string Phone { get; set; } = "";

        public string Role { get; set; } = "";
    }
}
