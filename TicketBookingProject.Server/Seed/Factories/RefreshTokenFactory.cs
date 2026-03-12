using Bogus;
using System.Security.Cryptography;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public static class RefreshTokenFactory
{
    private static readonly Faker _faker = new("vi");

    private static readonly string[] DeviceInfos =
    [
        "Chrome 120 / Windows 11",
        "Safari 17 / macOS Sonoma",
        "Firefox 121 / Ubuntu 22.04",
        "Chrome 120 / Android 14",
        "Safari 17 / iOS 17",
        "Postman / Windows 11",
    ];

    /// <summary>
    /// Token lưu DB là SHA-256 hash của raw token (không lưu plaintext).
    /// </summary>
    private static string HashToken(string raw)
    {
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(bytes).ToLower();
    }

    public static RefreshToken Create(
        int userId,
        RefreshTokenStatus status = RefreshTokenStatus.Active,
        int lifetimeDays = 30)
    {
        var raw = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        var createdAt = _faker.Date.Past(1).ToUniversalTime();
        var expiresAt = createdAt.AddDays(lifetimeDays);
        var isRevoked = status == RefreshTokenStatus.Revoked;

        return new RefreshToken
        {
            UserId = userId,
            Token = HashToken(raw),
            DeviceInfo = _faker.PickRandom(DeviceInfos),
            IpAddress = _faker.Internet.Ip(),
            Status = expiresAt < DateTime.UtcNow ? RefreshTokenStatus.Expired : status,
            ExpiresAt = expiresAt,
            RevokedAt = isRevoked ? createdAt.AddDays(_faker.Random.Int(1, lifetimeDays)) : null,
            CreatedAt = createdAt,
        };
    }

    /// <summary>Tạo token active cho user (thường dùng khi seed user login gần đây).</summary>
    public static RefreshToken CreateActive(int userId) =>
        Create(userId, RefreshTokenStatus.Active);

    /// <summary>Tạo token đã bị revoke (logout).</summary>
    public static RefreshToken CreateRevoked(int userId) =>
        Create(userId, RefreshTokenStatus.Revoked);

    /// <summary>Tạo token đã hết hạn (expired naturally).</summary>
    public static RefreshToken CreateExpired(int userId) =>
        Create(userId, RefreshTokenStatus.Active, lifetimeDays: -1); // expiresAt in the past
}
