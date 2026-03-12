using Microsoft.EntityFrameworkCore;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class RefreshTokenSeeder(TicketBookingProjectContext db, ILogger<RefreshTokenSeeder> logger)
{
    public async Task SeedAsync(
        List<User> admins,
        List<User> organizers,
        List<User> staff,
        List<User> customers)
    {
        if (await db.Set<RefreshToken>().AnyAsync())
        {
            logger.LogInformation("[RefreshTokenSeeder] Already seeded, skipping.");
            return;
        }

        var tokens = new List<RefreshToken>();

        // Admins — luôn có active token + 1 revoked (logout cũ)
        foreach (var user in admins)
        {
            tokens.Add(RefreshTokenFactory.CreateActive(user.Id));
            tokens.Add(RefreshTokenFactory.CreateRevoked(user.Id));
        }

        // Organizers — active token + đôi khi có expired
        foreach (var user in organizers)
        {
            tokens.Add(RefreshTokenFactory.CreateActive(user.Id));
            tokens.Add(RefreshTokenFactory.CreateExpired(user.Id));
        }

        // Staff — chỉ active token
        foreach (var user in staff)
        {
            tokens.Add(RefreshTokenFactory.CreateActive(user.Id));
        }

        // Customers — mix: active, revoked, expired
        foreach (var user in customers)
        {
            tokens.Add(RefreshTokenFactory.CreateActive(user.Id));

            // 40% có thêm revoked (đã logout thiết bị cũ)
            if (Random.Shared.NextDouble() < 0.4)
                tokens.Add(RefreshTokenFactory.CreateRevoked(user.Id));

            // 20% có thêm expired (token hết hạn tự nhiên)
            if (Random.Shared.NextDouble() < 0.2)
                tokens.Add(RefreshTokenFactory.CreateExpired(user.Id));
        }

        await db.Set<RefreshToken>().AddRangeAsync(tokens);
        await db.SaveChangesAsync();

        logger.LogInformation(
            "[RefreshTokenSeeder] Seeded {Count} refresh tokens for {Users} users.",
            tokens.Count,
            admins.Count + organizers.Count + staff.Count + customers.Count);
    }
}