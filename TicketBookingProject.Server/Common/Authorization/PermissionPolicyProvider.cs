using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace TicketBookingProject.Server;

public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : base(options) { }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = await base.GetPolicyAsync(policyName);
        if (policy != null) return policy;

        // Nếu Policy chưa tồn tại, tự động tạo một Policy mới dựa trên tên Permission
        return new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();
    }
}
