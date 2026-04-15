using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using TicketBookingProject.Server.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace TicketBookingProject.Server;

public class TicketRepository : BaseRepository<Ticket>, ITicketRepository
{
    public TicketRepository(TicketBookingProjectContext db) : base(db)
    {
    }

    public async Task<List<Ticket>> CreateTickets (BookingTicketDetails booking)
    {
        var now = DateTime.UtcNow;
        var tickets = booking.Details.SelectMany(d => Enumerable.Range(0, d.Quantity)
        .Select(_ => new Ticket
        {
            BookingId = booking.Id,
            TicketTypeId = d.TicketTypeId,
            Status = TicketStatus.Valid,
            CreatedAt = now,
            EventSeatId = d.EventSeatId,
            QrCode = Guid.NewGuid().ToString()
        })).ToList();

        _dbset.AddRange(tickets);
        await _db.SaveChangesAsync();

        return tickets;
    }

    //test cast với dto, nếu không được thì quay về query trực tiếp ở đây
    public async Task<List<Ticket>> GetTicketsByBookingId(int bookingId)
    {
        var tickets = await _dbset.Where(t => t.BookingId == bookingId)
            .Include(t => t.Booking)
                .ThenInclude(b => b.Event)
                    .ThenInclude(e => e.Venue)
            .Include(t => t.Booking)
                .ThenInclude(b => b.Event)
                    .ThenInclude(e => e.EventPosters)
            .Include(t => t.TicketType)
            .Include(t => t.EventSeat)
            .ToListAsync();

        return tickets;
    }

    public async Task<(IQueryable<Booking>, int TotalCount)> GetTicketsByUserId(int userId, TicketListRequest req)
    {
        var bookings = _db.Bookings
            .Where(b => b.UserId == userId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Search)) {
            bookings = bookings.Where(x => (x.Event != null &&
                                x.Event.Name != null &&
                                EF.Functions.Like(x.Event.Name ?? "", $"%{req.Search}%")) ||
                                (x.Event != null &&
                                x.Event.Venue != null && 
                                EF.Functions.Like(x.Event.Venue.Name ?? "", $"%{req.Search}%"))) ;
        }

        if (req.Status != null)
            bookings = bookings.Where(s => s.Status == req.Status);

        var totalCount = await bookings.CountAsync();

        if (req.SortDesc) bookings = bookings.OrderByDescending(x => x.CreatedAt);

        bookings = bookings
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize);

        return (bookings, totalCount);
    }
}
