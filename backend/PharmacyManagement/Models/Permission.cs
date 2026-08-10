using System.ComponentModel.DataAnnotations;

namespace PharmacyManagement.Models
{
    public class Permission
    {
        public long PermissionID { get; set; }

        [StringLength(255)]
        public String Name { get; set; }

        [StringLength(500)]
        public String? Description { get; set; }

        public ICollection<RolePermission>? RolePermission { get; set; }
    }
}
