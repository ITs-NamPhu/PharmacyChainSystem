namespace PharmacyManagement.DTOs.Auth
{
    public class LogoutRequest
    {
        public String accessToken { get; set; }
        public String refreshToken { get; set; }

    }
}
