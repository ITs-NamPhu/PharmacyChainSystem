using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace PharmacyManagement.Validators.PermissionHandle
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallbackProvider;

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith("Permission:", StringComparison.OrdinalIgnoreCase))
            {
                var permission = policyName.Substring("Permission:".Length);
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(permission));
                return Task.FromResult<AuthorizationPolicy?>(policy.Build());
            }

            return _fallbackProvider.GetPolicyAsync(policyName);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
            _fallbackProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
            _fallbackProvider.GetFallbackPolicyAsync();
    }
}
