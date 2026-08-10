namespace PharmacyManagement.DTOs.Auth
{
    public class JwtSetting
    {
        public string SecretKey { get; set; } = "";

        public string Issuer { get; set; } = "";

        public string Audience { get; set; } = "";

        public int ExpireMinutes { get; set; }
    }
}
