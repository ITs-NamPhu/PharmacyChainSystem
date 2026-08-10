namespace PharmacyManagement.DTOs.Auth
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = "";

        public string RefreshToken { get; set; } = "";

        public UserInfoResponse User { get; set; } = new();

        public List<LoginBranchResponse> Branches { get; set; } = new();
    }
}
