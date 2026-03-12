using Microsoft.AspNetCore.Authorization;

namespace TicketBookingProject.Server;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        // Lấy tất cả claim có loại là "permission" từ JWT
        var permissions = context.User.FindAll("permission").Select(x => x.Value);

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
