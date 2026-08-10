using PharmacyManagement.DTOs.Role;
using PharmacyManagement.Models;

namespace PharmacyManagement.Mappers
{
    public static class RoleMapper
    {
        public static Role ToEntity(this CreateRoleRequest request)
        {
            return new Role
            {
                RoleName = request.RoleName
            };
        }

        public static void ApplyTo(this UpdateRoleRequest request, Role entity)
        {
            entity.RoleName = request.RoleName;
        }

        public static RoleResponse ToResponse(this Role entity)
        {
            return new RoleResponse
            {
                RoleID = entity.RoleID,
                RoleName = entity.RoleName
            };
        }

        public static List<RoleResponse> ToResponseList(this List<Role> entities)
        {
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
