using Microsoft.AspNetCore.Authorization;

namespace TicketBookingProject.Server;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) : base(policy: permission) { }
}
