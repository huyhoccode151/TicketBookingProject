using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> CreateAsync(RefreshToken token, CancellationToken ct = default);
    Task<RefreshToken?> CheckToken(string hashedRfToken);
}
