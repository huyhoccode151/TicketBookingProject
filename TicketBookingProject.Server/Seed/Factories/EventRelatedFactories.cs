using Bogus;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public static class EventPosterFactory
{
    private static readonly Faker _faker = new("vi");

    public static List<EventPoster> CreateForEvent(int eventId, int count = 3)
    {
        var types = new[] { ImageType.Poster, ImageType.Banner, ImageType.Thumbnail };
        var posters = new List<EventPoster>();

        for (int i = 0; i < count; i++)
        {
            posters.Add(new EventPoster
            {
                EventId = eventId,
                ImageUrl = _faker.Image.PicsumUrl(1200, 630),
                ImageType = types[i % types.Length],
                IsPrimary = i == 0,
                CreatedAt = DateTime.UtcNow,
            });
        }

        return posters;
    }
}

// ─────────────────────────────────────────────
// TicketTypeFactory
// ─────────────────────────────────────────────
public static class TicketTypeFactory
{
    private static readonly (string Name, long Min, long Max)[] Templates =
    [
        ("Vé Thường",      200_000,   500_000),
        ("Vé VIP",         800_000, 2_000_000),
        ("Vé VVIP",      2_000_000, 5_000_000),
        ("Early Bird",     150_000,   350_000),
        ("Vé Standing",    100_000,   300_000),
        ("Vé Gia Đình",    400_000,   900_000),
        ("Vé Học Sinh/SV",  80_000,   200_000),
    ];

    private static readonly Faker _faker = new("vi");

    public static List<TicketType> CreateForEvent(int eventId, int count = 3)
    {
        var selected = Templates.OrderBy(_ => _faker.Random.Int()).Take(count);

        return selected.Select(t => new TicketType
        {
            EventId = eventId,
            Name = t.Name,
            Price = _faker.Random.Long(t.Min, t.Max),
            Quantity = _faker.Random.Int(50, 2_000),
            SoldQuantity = 0,
            MaxPerUser = _faker.Random.Int(1, 10),
            Status = TicketTypeStatus.OnSale,
            Version = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        }).ToList();
    }
}

// ─────────────────────────────────────────────
// EventSeatFactory
// ─────────────────────────────────────────────
public static class EventSeatFactory
{
    /// <summary>
    /// Maps venue seats → event_seats, distributing across ticket types round-robin.
    /// price = NULL means "use ticket_type.price".
    /// </summary>
    public static List<EventSeat> CreateFromSeats(
        int eventId,
        List<Seat> seats,
        List<TicketType> ticketTypes)
    {
        if (ticketTypes.Count == 0)
            throw new ArgumentException("At least one TicketType is required.", nameof(ticketTypes));

        return seats.Select((seat, idx) =>
        {
            var tt = ticketTypes[idx % ticketTypes.Count];
            return new EventSeat
            {
                EventId = eventId,
                SeatId = seat.Id,
                TicketTypeId = tt.Id,
                Price = null,                      // inherit from ticket type
                Status = EventSeatStatus.Available,
                Version = 0,
                UpdatedAt = DateTime.UtcNow,
            };
        }).ToList();
    }
}