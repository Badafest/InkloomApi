using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace InkloomApi.Authorization;

class AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    public DefaultAuthorizationPolicyProvider _defaultPolicyProvider = new(options);
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return _defaultPolicyProvider.GetDefaultPolicyAsync();
    }

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return _defaultPolicyProvider.GetFallbackPolicyAsync();
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        return Task.FromResult<AuthorizationPolicy?>(null);
    }
}