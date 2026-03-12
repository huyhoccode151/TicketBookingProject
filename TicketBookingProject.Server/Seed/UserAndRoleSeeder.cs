using AutoMapper.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class RoleSeeder(TicketBookingProjectContext db, ILogger<RoleSeeder> logger)
{
    public async Task<List<Role>> SeedAsync()
    {
        var existing = await db.Roles.ToListAsync();
        if (existing.Count > 0)
        {
            logger.LogInformation("[RoleSeeder] Already seeded ({Count} roles), skipping.", existing.Count);
            return existing;
        }

        await db.Roles.AddRangeAsync(RoleFactory.SystemRoles);
        await db.SaveChangesAsync();

        logger.LogInformation("[RoleSeeder] Seeded {Count} roles.", RoleFactory.SystemRoles.Count);
        return await db.Roles.ToListAsync();
    }
}

// ──────────────────────────────────────────────────────────
// UserSeeder
// ──────────────────────────────────────────────────────────
public class UserSeeder(
    TicketBookingProjectContext db,
    IOptions<SeedOptions> opts,
    ILogger<UserSeeder> logger)
{
    public async Task<(List<User> Admins, List<User> Organizers, List<User> Staff, List<User> Customers)>
        SeedAsync(List<Role> roles)
    {
        if (await db.Users.AnyAsync())
        {
            logger.LogInformation("[UserSeeder] Already seeded, skipping.");
            var all = await db.Users.ToListAsync();
            var o2 = opts.Value;
            return (
                all.Take(o2.AdminCount).ToList(),
                all.Skip(o2.AdminCount).Take(o2.OrganizerCount).ToList(),
                all.Skip(o2.AdminCount + o2.OrganizerCount).Take(o2.StaffCount).ToList(),
                all.Skip(o2.AdminCount + o2.OrganizerCount + o2.StaffCount).ToList()
            );
        }

        var o = opts.Value;

        var adminRole = roles.First(r => r.Name == "admin");
        var organizerRole = roles.First(r => r.Name == "organizer");
        var staffRole = roles.First(r => r.Name == "staff");
        var customerRole = roles.First(r => r.Name == "customer");

        var admins = new List<User> { UserFactory.CreateAdmin() };
        var organizers = Enumerable.Range(0, o.OrganizerCount).Select(_ => UserFactory.CreateOrganizer()).ToList();
        var staff = Enumerable.Range(0, o.StaffCount).Select(_ => UserFactory.CreateStaff()).ToList();
        var customers = Enumerable.Range(0, o.CustomerCount).Select(_ => UserFactory.CreateCustomer()).ToList();

        // Assign roles via navigation property (EF Core many-to-many, no join model needed)
        foreach (var u in admins) u.Roles = [adminRole];
        foreach (var u in organizers) u.Roles = [organizerRole];
        foreach (var u in staff) u.Roles = [staffRole];
        foreach (var u in customers) u.Roles = [customerRole];

        var allUsers = admins.Concat(organizers).Concat(staff).Concat(customers).ToList();
        await db.Users.AddRangeAsync(allUsers);
        await db.SaveChangesAsync();

        logger.LogInformation(
            "[UserSeeder] Seeded admins:{A} organizers:{O} staff:{S} customers:{C}",
            admins.Count, organizers.Count, staff.Count, customers.Count);

        return (admins, organizers, staff, customers);
    }
}