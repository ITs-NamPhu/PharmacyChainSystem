using Microsoft.AspNetCore.Authorization;

namespace PharmacyManagement.Validators.PermissionHandle
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public string Permission { get; }

        public HasPermissionAttribute(string permission)
        {
            Permission = permission;
            Policy = $"Permission:{permission}";
        }
    }
}
