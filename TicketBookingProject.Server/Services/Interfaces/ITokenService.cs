using System.Security.Claims;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface ITokenService
{
    string CreateAccessToken(User user, List<string> permissions, List<string> roles);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
