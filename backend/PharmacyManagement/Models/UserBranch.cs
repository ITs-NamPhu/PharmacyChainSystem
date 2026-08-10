namespace PharmacyManagement.Models
{
    public class UserBranch
    {
        public long UserBranchID { get; set; }

        public long UserID { get; set; }

        public long BranchID { get; set; }

        public long RoleID { get; set; }

        public bool IsDefault { get; set; }

        public User? User { get; set; }
        public Branch? Branch { get; set; }
        public Role? Role { get; set; }

    }
}
