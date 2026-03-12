using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class CategorySeeder(TicketBookingProjectContext db, ILogger<CategorySeeder> logger)
{
    public async Task<List<Category>> SeedAsync()
    {
        var existing = await db.Categories.ToListAsync();
        if (existing.Count > 0)
        {
            logger.LogInformation("[CategorySeeder] Already seeded ({Count}), skipping.", existing.Count);
            return existing;
        }

        await db.Categories.AddRangeAsync(CategoryFactory.SystemCategories);
        await db.SaveChangesAsync();

        logger.LogInformation("[CategorySeeder] Seeded {Count} categories.", CategoryFactory.SystemCategories.Count);
        return await db.Categories.ToListAsync();
    }
}

// ──────────────────────────────────────────────────────────
// VenueSeeder  →  venues → sections → seats (grid)
// ──────────────────────────────────────────────────────────
public class VenueSeeder(
    TicketBookingProjectContext db,
    IOptions<SeedOptions> opts,
    ILogger<VenueSeeder> logger)
{
    public async Task<List<Venue>> SeedAsync()
    {
        if (await db.Venues.AnyAsync())
        {
            logger.LogInformation("[VenueSeeder] Already seeded, skipping.");
            return await db.Venues.ToListAsync();
        }

        var o = opts.Value;
        var venues = VenueFactory.CreateMany(o.VenueCount);

        await db.Venues.AddRangeAsync(venues);
        await db.SaveChangesAsync();

        foreach (var venue in venues)
        {
            var sections = VenueSectionFactory.CreateForVenue(venue.Id, o.SectionsPerVenue);
            await db.VenueSections.AddRangeAsync(sections);
            await db.SaveChangesAsync();

            foreach (var section in sections)
            {
                // Assign seat type per section: first section = VIP, last = Standing, rest = Normal
                var seatType = sections.IndexOf(section) == 0 ? SeatType.Vip
                             : sections.IndexOf(section) == sections.Count - 1 ? SeatType.Standing
                             : SeatType.Normal;

                var seats = SeatFactory.CreateGrid(venue.Id, section.Id,
                                o.RowsPerSection, o.SeatsPerRow, seatType);

                await db.Seats.AddRangeAsync(seats);
            }

            await db.SaveChangesAsync();
        }

        var totalSeats = o.VenueCount * o.SectionsPerVenue * o.RowsPerSection * o.SeatsPerRow;
        logger.LogInformation(
            "[VenueSeeder] Seeded {V} venues, {S} sections, {T} seats.",
            o.VenueCount, o.VenueCount * o.SectionsPerVenue, totalSeats);

        return await db.Venues.ToListAsync();
    }
}
