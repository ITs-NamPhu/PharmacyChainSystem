using System.ComponentModel.DataAnnotations;

namespace PharmacyManagement.DTOs.User
{
    public class UserResponse
    {
        public long UserID { get; set; }

        [StringLength(255)]
        public String FullName { get; set; }
        [StringLength(10)]
        public String Phone { get; set; }
        [StringLength(255)]
        public String? Address { get; set; }
        [StringLength(255)]
        public String? Email { get; set; }

        public Boolean IsActive { get; set; }

        public long RoleID { get; set; }

        public String? RoleName { get; set; }

        public long BranchID { get; set; }
    }
}
