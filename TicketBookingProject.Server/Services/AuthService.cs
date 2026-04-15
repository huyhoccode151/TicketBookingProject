using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TicketBookingProject.Server.Enums;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _rfToken;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _cfg;
    private readonly IAuditLogService _auditLog;
    public AuthService(IUserRepository users, IRefreshTokenRepository rfToken, ITokenService tokenService, IConfiguration cfg, IAuditLogService auditLog)
    {
        _users = users;
        _rfToken = rfToken;
        _tokenService = tokenService;
        _cfg = cfg;
        _auditLog = auditLog;
    }

    public async Task<TokenResponse> Login(LoginRequest user, string? ip = null, CancellationToken ct = default)
    {
        var foundUser = await _users.GetByUserNameAsync(user.UsernameOrEmail); //tam thoi tim theo username
        if (foundUser == null) throw new Exception("Tài khoản không tồn tại");

        if (foundUser.LockoutEnd < DateTime.UtcNow) throw new Exception("Tài khoản bị khóa. " +
            $"Hãy thử lại sau {Math.Ceiling((foundUser.LockoutEnd.Value - DateTime.UtcNow).TotalMinutes)} phút.");

        if (!PasswordHasher.Verify(user.Password, foundUser.Password))
        {
            foundUser.AccessFailedCount++;
            if (foundUser.AccessFailedCount > 5)
            {
                foundUser.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                foundUser.AccessFailedCount = 0;
            }
            await _users.UpdateAsync(foundUser);
            throw new Exception("Mật khẩu không chính xác");
        }

        foundUser.AccessFailedCount = 0;
        foundUser.LockoutEnd = null;

        var permissions = await _users.GetPermissionsByUserIdAsync(foundUser.Id, ct);

        var accessToken = _tokenService.CreateAccessToken(foundUser, permissions);
        var refreshToken = GenerateRefreshToken();
        string hashedRefreshToken = HashSHA256(refreshToken);

        await _rfToken.CreateAsync(new RefreshToken
        {
            UserId = foundUser.Id,
            Token = hashedRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            Status = RefreshTokenStatus.Active,
            IpAddress = ip,
        },ct);

        _auditLog.AddLog(
            AuditAction.Login,
            "Login",
            foundUser.Id,
            $"Tài khoản {foundUser.Username} đăng nhập thành công",
            new
            {
                foundUser.Username,
                foundUser.Email,
            },
            foundUser.Id
            );
        await _users.SaveChanges();

        return new TokenResponse { AccessToken = accessToken, RefreshToken = refreshToken };
    }

    private string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private string HashSHA256(string input)
    {
        using var sha256 = SHA256.Create();
        return Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(input)));
    }

    public async Task<TokenResponse> RefreshToken(string tokenRes, string? ip = null, CancellationToken ct = default)
    {
        var hashedRfToken = HashSHA256(tokenRes);

        // 2. Kiểm tra Refresh Token trong Database
        var storedToken = await _rfToken.CheckToken(hashedRfToken);
        if (storedToken == null) throw new Exception("Refresh Token hết hạn hoặc không tồn tại");

        // 3. Vô hiệu hóa Token cũ (Security Best Practice: Rotation)
        storedToken.Status = RefreshTokenStatus.Revoked; // Đánh dấu là đã sử dụng/revoked
        storedToken.RevokedAt = DateTime.UtcNow;

        // 4. Lấy lại danh sách Permission của User để tạo Access Token mới
        var userId = storedToken.UserId;
        var user = await _users.GetByIdAsync(userId);
        if (user == null) throw new Exception("User không tồn tại");

        var permissions = await _users.GetPermissionsByUserIdAsync(userId);

        // 5. Tạo cặp Token mới
        var newAccessToken = _tokenService.CreateAccessToken(user, permissions);
        var newRefreshToken = GenerateRefreshToken();

        // 6. Lưu Refresh Token mới vào DB
        int rfDays = _cfg.GetValue<int>("Jwt:RefreshTokenDays", 7);
        await _rfToken.CreateAsync(new RefreshToken
        {
            UserId = userId,
            Token = HashSHA256(newRefreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(rfDays),
            CreatedAt = DateTime.UtcNow,
            Status = RefreshTokenStatus.Active,
            IpAddress = ip,
        }, ct);

        return new TokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
}
