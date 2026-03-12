namespace TicketBookingProject.Server;

public interface IAuthService
{
    Task<TokenResponse> Login(LoginRequest user, string? ip = null, CancellationToken ct = default);

    Task<TokenResponse> RefreshToken(string tokenRes, string? ip = null, CancellationToken ct = default);
}
