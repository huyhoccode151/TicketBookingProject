using Microsoft.EntityFrameworkCore;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class PermissionSeeder(TicketBookingProjectContext db, ILogger<PermissionSeeder> logger)
{
    public async Task SeedAsync(List<Role> roles)
    {
        if (await db.Set<Permission>().AnyAsync())
        {
            logger.LogInformation("[PermissionSeeder] Already seeded, skipping.");
            return;
        }

        // 1. Insert all permissions
        var permissions = PermissionFactory.AllPermissions;
        await db.Set<Permission>().AddRangeAsync(permissions);
        await db.SaveChangesAsync();

        logger.LogInformation("[PermissionSeeder] Seeded {Count} permissions.", permissions.Count);

        // 2. Gán permissions cho từng role qua navigation property
        //    EF Core tự insert vào role_permissions
        var permByName = permissions.ToDictionary(p => p.Name);

        foreach (var role in roles)
        {
            if (!PermissionFactory.RolePermissions.TryGetValue(role.Name, out var assigned))
                continue;

            // Attach đúng permission instance đã được EF track
            role.Permissions = assigned
                .Select(p => permByName[p.Name])
                .ToList();
        }

        await db.SaveChangesAsync();

        foreach (var role in roles)
        {
            logger.LogInformation(
                "[PermissionSeeder] Role '{Role}' → {Count} permissions.",
                role.Name, role.Permissions.Count);
        }
    }
}
