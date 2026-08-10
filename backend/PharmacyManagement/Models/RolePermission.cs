namespace PharmacyManagement.Models
{
    public class RolePermission
    {
        public long RolePermissionID { get; set; }

        public long RoleID { get; set; }

        public long PermissionID { get; set; }

        public Role? Role { get; set; }
        public Permission? Permission { get; set; }
    }
}
