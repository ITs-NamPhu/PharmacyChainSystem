using Microsoft.AspNetCore.Authorization;

namespace PharmacyManagement.Validators.PermissionHandle
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
