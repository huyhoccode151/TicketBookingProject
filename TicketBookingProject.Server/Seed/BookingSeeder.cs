using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

/// <summary>
/// Seeds: bookings → booking_details → tickets
///              → seat_holds (pending) → payments → refunds → event_seat_logs
/// </summary>
public class BookingSeeder(
    TicketBookingProjectContext db,
    IOptions<SeedOptions> opts,
    ILogger<BookingSeeder> logger)
{
    private readonly Faker _faker = new("vi");

    public async Task SeedAsync(List<User> customers, List<Event> events, List<User> admins, List<User> staff)
    {
        if (await db.Bookings.AnyAsync())
        {
            logger.LogInformation("[BookingSeeder] Already seeded, skipping.");
            return;
        }

        var o = opts.Value;
        var adminId = admins.First().Id;

        foreach (var ev in events)
        {
            var availableSeats = await db.EventSeats
                .Where(es => es.EventId == ev.Id && es.Status == EventSeatStatus.Available)
                .ToListAsync();

            var ticketTypes = await db.TicketTypes
                .Where(tt => tt.EventId == ev.Id)
                .ToDictionaryAsync(tt => tt.Id);

            if (availableSeats.Count == 0 || ticketTypes.Count == 0) continue;

            var seatQueue = new Queue<EventSeat>(availableSeats.OrderBy(_ => _faker.Random.Int()));
            var staffUser = _faker.PickRandom(staff);

            await SeedBookingGroup(ev, customers, seatQueue, ticketTypes,
                o.ConfirmedBookingsPerEvent, o.MaxSeatsPerBooking,
                BookingStatus.Confirmed, adminId, staffUser.Id);

            await SeedBookingGroup(ev, customers, seatQueue, ticketTypes,
                o.PendingBookingsPerEvent, o.MaxSeatsPerBooking,
                BookingStatus.Pending, adminId, staffUser.Id);

            await SeedBookingGroup(ev, customers, seatQueue, ticketTypes,
                o.CancelledBookingsPerEvent, o.MaxSeatsPerBooking,
                BookingStatus.Cancelled, adminId, staffUser.Id,
                refundRatio: o.RefundRatio);
        }

        logger.LogInformation("[BookingSeeder] Seeded bookings for {Count} events.", events.Count);
    }

    // ──────────────────────────────────────────────────────────
    private async Task SeedBookingGroup(
        Event ev,
        List<User> customers,
        Queue<EventSeat> seatQueue,
        Dictionary<int, TicketType> ticketTypes,
        int count,
        int maxSeatsPerBooking,
        BookingStatus status,
        int adminId,
        int staffId,
        double refundRatio = 0)
    {
        for (int i = 0; i < count && seatQueue.Count > 0; i++)
        {
            var user = _faker.PickRandom(customers);
            var seatCount = Math.Min(_faker.Random.Int(1, maxSeatsPerBooking), seatQueue.Count);
            var seats = Enumerable.Range(0, seatCount)
                                      .Select(_ => seatQueue.Dequeue())
                                      .ToList();

            // Resolve price per seat
            var items = seats.Select(es =>
            {
                var price = es.Price
                    ?? (ticketTypes.TryGetValue(es.TicketTypeId, out var tt) ? tt.Price : 0L);
                return (Seat: es, Price: price);
            }).ToList();

            var totalAmount = items.Sum(x => x.Price);

            // ── Booking ──────────────────────────────────────
            var booking = BookingFactory.Create(user.Id, ev.Id, totalAmount, status);
            await db.Bookings.AddAsync(booking);
            await db.SaveChangesAsync();

            // ── Booking Details ──────────────────────────────
            var details = BookingDetailFactory.CreateFromEventSeats(
                booking.Id,
                items.Select(x => (x.Seat, x.Price)).ToList());
            await db.BookingDetails.AddRangeAsync(details);

            // ── EventSeat status + Logs ──────────────────────
            var newSeatStatus = status switch
            {
                BookingStatus.Confirmed => EventSeatStatus.Sold,
                BookingStatus.Pending => EventSeatStatus.Reserved,
                _ => EventSeatStatus.Available,
            };

            var logs = new List<EventSeatLog>();
            foreach (var es in seats)
            {
                var oldStatus = es.Status;
                es.Status = newSeatStatus;
                es.Version += 1;
                es.UpdatedAt = DateTime.UtcNow;

                var action = status switch
                {
                    BookingStatus.Confirmed => "confirm",
                    BookingStatus.Pending => "hold",
                    _ => "cancel",
                };
                logs.Add(EventSeatLogFactory.Create(
                    es.Id, oldStatus, newSeatStatus, action, booking.Id, user.Id));
            }
            await db.EventSeatLogs.AddRangeAsync(logs);
            await db.SaveChangesAsync();

            // ── Status-specific entities ─────────────────────
            switch (status)
            {
                case BookingStatus.Confirmed:
                    // Tickets — some marked as Used (checked-in)
                    var ticketStatus = _faker.Random.Bool(0.3f)
                        ? TicketStatus.Used
                        : TicketStatus.Valid;
                    var tickets = TicketFactory.CreateFromDetails(
                        booking.Id, details, ticketStatus,
                        checkedInBy: ticketStatus == TicketStatus.Used ? staffId : null);
                    await db.Tickets.AddRangeAsync(tickets);

                    // Increment SoldQuantity on TicketTypes
                    foreach (var es in seats)
                    {
                        if (ticketTypes.TryGetValue(es.TicketTypeId, out var tt))
                        {
                            tt.SoldQuantity += 1;
                            tt.Version += 1;
                        }
                    }

                    await db.Payments.AddAsync(
                        PaymentFactory.CreateSuccess(booking.Id, user.Id, totalAmount));
                    break;

                case BookingStatus.Pending:
                    // SeatHolds for each reserved seat
                    foreach (var es in seats)
                    {
                        await db.SeatHolds.AddAsync(
                            SeatHoldFactory.Create(es.Id, user.Id, booking.Id, SeatHoldStatus.Active));
                    }

                    await db.Payments.AddAsync(
                        PaymentFactory.CreatePending(booking.Id, user.Id, totalAmount));
                    break;

                case BookingStatus.Cancelled:
                    await db.Payments.AddAsync(
                        PaymentFactory.CreateFailed(booking.Id, user.Id, totalAmount));
                    await db.SaveChangesAsync();

                    // Refund for a portion of cancelled bookings
                    if (_faker.Random.Double() < refundRatio)
                    {
                        var payment = await db.Payments
                            .FirstAsync(p => p.BookingId == booking.Id);

                        await db.Refunds.AddAsync(
                            RefundFactory.Create(
                                payment.Id, booking.Id, totalAmount,
                                RefundStatus.Completed, adminId));
                    }
                    break;
            }

            await db.SaveChangesAsync();
        }
    }
}
