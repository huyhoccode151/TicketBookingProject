using System.Security.Claims;

namespace TicketBookingProject.Server;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _context;

    public CurrentUserService(IHttpContextAccessor context)
    {
        _context = context;
    }

    //public int UserId => int.Parse(_context.HttpContext?.User?.FindFirst("userId")?.Value);
    public int? UserId
    {
        get
        {
            var value = _context.HttpContext?.User?.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(value))
                throw new UnauthorizedAccessException("UserId not found in claims");

            return int.Parse(value);
        }
    }

    public string? UserName => _context.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

    public List<string>? Permission => _context.HttpContext?.User?.FindAll("permission")
        .Select(x => x.Value)
        .ToList();
}
