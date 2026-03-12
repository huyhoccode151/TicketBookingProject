using Bogus;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public static class EventFactory
{
    private static readonly string[] Prefixes =
        ["Đại nhạc hội", "Concert", "Festival", "Gala", "Show", "Đêm nhạc", "Live Show"];

    private static readonly Faker<Event> _faker = new Faker<Event>("vi")
        .RuleFor(e => e.Name, f => $"{f.PickRandom(Prefixes)} {f.Name.FirstName()} {f.Date.Future().Year}")
        .RuleFor(e => e.Description, f => f.Lorem.Paragraphs(2))
        .RuleFor(e => e.Status, f => EventStatus.Published)
        .RuleFor(e => e.MaxTicketsPerBooking, f => f.Random.Int(1, 10))
        .RuleFor(e => e.CreatedAt, f => f.Date.Past(1).ToUniversalTime())
        .RuleFor(e => e.UpdatedAt, (f, e) => e.CreatedAt)
        .RuleFor(e => e.DeletedAt, f => null);

    public static Event Create(
        int organizerId,
        int venueId,
        int categoryId,
        Action<Event>? overrides = null)
    {
        var ev = _faker.Generate();
        ev.OrganizerId = organizerId;
        ev.VenueId = venueId;
        ev.CategoryId = categoryId;

        var baseDate = DateTime.UtcNow.AddDays(new Faker().Random.Int(7, 180));
        ev.ActiveAt = baseDate;
        ev.EndAt = baseDate.AddHours(new Faker().Random.Int(2, 6));
        ev.SaleStartAt = baseDate.AddDays(-30);
        ev.SaleEndAt = baseDate.AddHours(-1);

        overrides?.Invoke(ev);
        return ev;
    }

    public static List<Event> CreateMany(
        int count,
        int organizerId,
        int venueId,
        int categoryId,
        Action<Event>? overrides = null)
        => Enumerable.Range(0, count)
                     .Select(_ => Create(organizerId, venueId, categoryId, overrides))
                     .ToList();

    public static Event CreatePast(int organizerId, int venueId, int categoryId)
    {
        var ev = Create(organizerId, venueId, categoryId);
        var past = DateTime.UtcNow.AddDays(-new Faker().Random.Int(30, 365));
        ev.ActiveAt = past;
        ev.EndAt = past.AddHours(4);
        ev.SaleStartAt = past.AddDays(-30);
        ev.SaleEndAt = past.AddHours(-1);
        ev.Status = EventStatus.Completed;
        return ev;
    }
}
