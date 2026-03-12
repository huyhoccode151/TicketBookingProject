using Microsoft.Identity.Client;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(TicketBookingProjectContext db) : base(db)
    {
    }
    public async Task<RefreshToken> CreateAsync(RefreshToken token, CancellationToken ct = default)
    {
        _db.RefreshTokens.Add(token);
        await _db.SaveChangesAsync(ct);
        return token;
    }
    public async Task<RefreshToken?> CheckToken(string hashedRfToken)
    {
        var storedToken = _db.RefreshTokens.FirstOrDefault(rt => rt.Token == hashedRfToken && rt.IsActive);
        return storedToken;
    }
}
