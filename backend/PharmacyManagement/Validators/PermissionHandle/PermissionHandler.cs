using Microsoft.AspNetCore.Authorization;
using PharmacyManagement.Services.Interfaces;

namespace PharmacyManagement.Validators.PermissionHandle
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IServiceProvider _serviceProvider;

        public PermissionHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            Console.WriteLine("userid:{0}", userIdClaim);
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

            var permissions = await permissionService.GetPermission(userId);

            Console.WriteLine("===== CLAIMS =====");

            foreach (var c in context.User.Claims)
            {
                Console.WriteLine($"{c.Type} = {c.Value}");
            }

            Console.WriteLine("==================");

            Console.WriteLine("Permissions: " + string.Join(", ", permissions));

            foreach (var p in permissions)
            {
                Console.WriteLine(
                    $"{p} == {requirement.Permission} : " +
                    string.Equals(
                        p,
                        requirement.Permission,
                        StringComparison.OrdinalIgnoreCase));
            }
            if (permissions.Contains(requirement.Permission, StringComparer.OrdinalIgnoreCase))
            {
                context.Succeed(requirement);
            }

            Console.WriteLine("Pending requirements:");

            foreach (var r in context.PendingRequirements)
            {
                Console.WriteLine(r.GetType().Name);
            }
        }
    }
}
