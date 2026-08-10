namespace PharmacyManagement.DTOs.Auth
{
    public class LoginBranchResponse
    {
        public long BranchId { get; set; }

        public string BranchName { get; set; } = "";

        public bool IsDefault { get; set; }
    }
}
