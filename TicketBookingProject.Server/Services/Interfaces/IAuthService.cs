namespace TicketBookingProject.Server;

public interface IAuthService
{
    Task<Result<LoginResponse>> Login(LoginRequest user, string? ip = null, CancellationToken ct = default);
    Task<Result<LoginResponse>> GoogleLogin(GoogleLoginRequest request, string? ip = null, CancellationToken ct = default);

    Task<TokenResponse> RefreshToken(string tokenRes, string? ip = null, CancellationToken ct = default);
}
