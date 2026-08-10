namespace PharmacyManagement.DTOs.User
{
    public class UserListResponse
    {
        public int NumRecords { get; set; }

        public float TotalPage { get; set; }

        public List<UserResponse> Users { get; set; } = new();
    }
}
