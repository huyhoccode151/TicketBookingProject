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

    //init tickets
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

    //cancel tickets
    public async Task<bool> CancelTicket(List<int> bookingIds)
    {
        var tickets = _dbset.Where(t => bookingIds.Contains(t.BookingId)).ToList();

        if (tickets.Count() == 0) return false;

        tickets.ForEach(t => t.Status = TicketStatus.Cancelled);

        return true;
    }

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

    public async Task<Ticket?> GetTicketByIdAsync(int ticketId)
    {
        return await _dbset
            .Include(t => t.Booking)
                .ThenInclude(b => b.Event)
                    .ThenInclude(e => e.Venue)
            .Include(t => t.Booking)
                .ThenInclude(b => b.Event)
                    .ThenInclude(e => e.EventPosters)
            .Include(t => t.TicketType)
            .Include(t => t.EventSeat)
            .FirstOrDefaultAsync(t => t.Id == ticketId);
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

    public async Task<IQueryable<Booking>> GetUpcomingTicketsByUserId(int userId)
    {
        var bookings = _db.Bookings.Where(b => b.UserId == userId).AsQueryable();

        bookings = bookings.Where(b => b.Event.ActiveAt >= DateTime.UtcNow && b.Status == BookingStatus.Confirmed).OrderByDescending(b => b.Event.ActiveAt).ThenByDescending(b => b.CreatedAt);
        return bookings;
    }

    public async Task<Ticket?> GetByQrCodeAsync(string qrCode)
    {
        return await _dbset
            .Include(t => t.EventSeat)
            .Include(t => t.TicketType)
            .Include(t => t.Booking)
                .ThenInclude(b => b.User)
            .FirstOrDefaultAsync(t => t.QrCode == qrCode);
    }

    public async Task<bool> UpdateTicketAsync(Ticket ticket)
    {
        _dbset.Update(ticket);
        return await _db.SaveChangesAsync() > 0;
    }
}
