using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

/// <summary>
/// Seeds: events → ticket_types → event_posters → event_seats
/// </summary>
public class EventSeeder(
    TicketBookingProjectContext db,
    IOptions<SeedOptions> opts,
    ILogger<EventSeeder> logger)
{
    private readonly Faker _faker = new("vi");

    public async Task<List<Event>> SeedAsync(
        List<User> organizers,
        List<Venue> venues,
        List<Category> categories)
    {
        if (await db.Events.AnyAsync())
        {
            logger.LogInformation("[EventSeeder] Already seeded, skipping.");
            return await db.Events.ToListAsync();
        }

        var o = opts.Value;

        foreach (var organizer in organizers)
        {
            var venue = _faker.PickRandom(venues);
            var category = _faker.PickRandom(categories);

            var events = EventFactory.CreateMany(
                o.EventsPerOrganizer, organizer.Id, venue.Id, category.Id);

            await db.Events.AddRangeAsync(events);
            await db.SaveChangesAsync();

            foreach (var ev in events)
            {
                // ── Ticket Types ─────────────────────────────────
                var ticketTypes = TicketTypeFactory.CreateForEvent(ev.Id, o.TicketTypesPerEvent);
                await db.TicketTypes.AddRangeAsync(ticketTypes);
                await db.SaveChangesAsync();

                // ── Posters ──────────────────────────────────────
                var posters = EventPosterFactory.CreateForEvent(ev.Id, o.PostersPerEvent);
                await db.EventPosters.AddRangeAsync(posters);

                // ── Event Seats ──────────────────────────────────
                var venueSeats = await db.Seats
                    .Where(s => s.VenueId == ev.VenueId)
                    .ToListAsync();

                var eventSeats = EventSeatFactory.CreateFromSeats(ev.Id, venueSeats, ticketTypes);
                await db.EventSeats.AddRangeAsync(eventSeats);

                await db.SaveChangesAsync();
            }
        }

        var total = await db.Events.CountAsync();
        logger.LogInformation("[EventSeeder] Seeded {Count} events.", total);

        return await db.Events.ToListAsync();
    }
}
