namespace PharmacyManagement.DTOs.User
{
    public class CreateUserRequest
    {
        public string UserName { get; set; } = "";

        public string Password { get; set; } = "";

        public string FullName { get; set; } = "";

        public string Phone { get; set; } = "";

        public string Address { get; set; } = "";

        public string Email { get; set; } = "";
    }

    public class UpdateUserRequest
    {
        public string FullName { get; set; } = "";

        public string Phone { get; set; } = "";

        public string Address { get; set; } = "";

        public string Email { get; set; } = "";

        public bool IsActive { get; set; }
    }

    public class UserAssignmentRequest
    {
        public long RoleId { get; set; }
    }
}
