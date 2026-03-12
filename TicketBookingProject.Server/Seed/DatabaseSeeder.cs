using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

// ──────────────────────────────────────────────────────────
// DatabaseSeeder  —  master orchestrator
// ──────────────────────────────────────────────────────────
public class DatabaseSeeder(
    TicketBookingProjectContext db,
    RoleSeeder roleSeeder,
    UserSeeder userSeeder,
    PermissionSeeder permissionSeeder,
    CategorySeeder categorySeeder,
    VenueSeeder venueSeeder,
    EventSeeder eventSeeder,
    BookingSeeder bookingSeeder,
    RefreshTokenSeeder refreshTokenSeeder,
    ILogger<DatabaseSeeder> logger)
{
    public async Task SeedAsync()
    {
        logger.LogInformation("══════ Database seeding started ══════");

        // 1. Roles  (no FK dependency)
        var roles = await roleSeeder.SeedAsync();

        // 2. Users  (depends on: roles)
        var (admins, organizers, staff, customers) = await userSeeder.SeedAsync(roles);

        // 3. Permissions → role_permissions  (depends on: roles)
        await permissionSeeder.SeedAsync(roles);

        // 4. Categories  (no FK dependency)
        var categories = await categorySeeder.SeedAsync();

        // 5. RefreshTokens  (depends on: users)
        await refreshTokenSeeder.SeedAsync(admins, organizers, staff, customers);

        // 6. Venues → Sections → Seats  (no FK dependency)
        var venues = await venueSeeder.SeedAsync();

        // 7. Events → TicketTypes → Posters → EventSeats  (depends on: organizers, venues, categories)
        var events = await eventSeeder.SeedAsync(organizers, venues, categories);

        // 8. Bookings → BookingDetails → Tickets → SeatHolds → Payments → Refunds → SeatLogs
        //    (depends on: customers, events, admins, staff)
        await bookingSeeder.SeedAsync(customers, events, admins, staff);



        logger.LogInformation("══════ Database seeding completed ══════");
    }
}

// ──────────────────────────────────────────────────────────
// DI Registration
// ──────────────────────────────────────────────────────────
public static class SeederServiceExtensions
{
    /// <summary>
    /// Register all seeders and SeedOptions.
    /// Call in Program.cs: builder.Services.AddSeeders(builder.Configuration);
    /// </summary>
    public static IServiceCollection AddSeeders(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SeedOptions>(configuration.GetSection("SeedOptions"));

        services.AddScoped<RoleSeeder>();
        services.AddScoped<UserSeeder>();
        services.AddScoped<PermissionSeeder>();
        services.AddScoped<CategorySeeder>();
        services.AddScoped<VenueSeeder>();
        services.AddScoped<EventSeeder>();
        services.AddScoped<BookingSeeder>();
        services.AddScoped<RefreshTokenSeeder>();
        services.AddScoped<DatabaseSeeder>();

        return services;
    }
}
