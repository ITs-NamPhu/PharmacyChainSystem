namespace PharmacyManagement.DTOs.Role
{
    public class RoleListResponse
    {
        public int NumRecords { get; set; }
        public float TotalPage { get; set; }
        public List<RoleResponse> Roles { get; set; } = new();
    }
}
