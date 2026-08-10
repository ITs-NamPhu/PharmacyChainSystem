using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace PharmacyManagement.Models
{
	public class Role
	{
		public long RoleID { get; set; }
        [StringLength(255)]
        public String RoleName { get; set; }

        public ICollection<UserBranch>? UserBranch { get; set; }
        public ICollection<RolePermission>? RolePermission { get; set; }
    }
}
