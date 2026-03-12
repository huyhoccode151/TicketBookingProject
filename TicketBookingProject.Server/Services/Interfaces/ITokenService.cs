using System.Security.Claims;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface ITokenService
{
    string CreateAccessToken(User user, List<string> permissions);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
