using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Text.Json;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class BookingRepository : BaseRepository<Booking>, IBookingRepository
{
    public BookingRepository(TicketBookingProjectContext context) : base(context)
    {
    }

    public async Task<BookingResponse> CreateBookingAsync(int userId, List<SeatHold> holdResult)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            if (holdResult == null || !holdResult.Any())
                throw new Exception("No seat holds");

            var ticketTypeIds = holdResult
                .Select(h => h.TicketTypeId)
                .Distinct()
                .ToList();

            var ticketTypes = await _db.TicketTypes
                .Where(t => ticketTypeIds.Contains(t.Id))
                .Select(t => new
                {
                    t.Id,
                    t.EventId,
                    t.Price
                })
                .ToListAsync();

            var eventId = ticketTypes.First().EventId;

            var booking = new Booking
            {
                UserId = userId,
                EventId = eventId,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                BookingDetails = holdResult.Select(h =>
                {
                    var ticket = ticketTypes.First(t => t.Id == h.TicketTypeId);

                    return new BookingDetail
                    {
                        EventSeatId = h.EventSeatId,
                        TicketTypeId = h.TicketTypeId,
                        Price = ticket.Price,
                        Quantity = h.Quantity
                    };
                }).ToList()
            };

            booking.TotalAmount = booking.BookingDetails.Sum(d => d.Price);

            await _dbset.AddAsync(booking);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            return new BookingResponse
            {
                Id = booking.Id,
                UserId = booking.UserId,
                EventId = booking.EventId,
                Status = booking.Status.ToString(),
                CreatedAt = booking.CreatedAt,
                Details = booking.BookingDetails.Select(d => new BookingDetailResponse
                {
                    EventSeatId = d.EventSeatId,
                    TicketTypeId = d.TicketTypeId,
                    Price = d.Price,
                    Quantity = d.Quantity
                }).ToList()
            };
        }
        catch 
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    public async Task<BookingTicketDetails?> GetBookingByIdAsync(int id)
    {
        return await _dbset
        .AsNoTracking()
        .Where(b => b.Id == id)
        .Include(b => b.SeatHolds)
        .Include(b => b.BookingDetails)
            .ThenInclude(d => d.TicketType)
        .Select(b => new BookingTicketDetails
        {
            Id = b.Id,
            UserId = b.UserId,
            EventId = b.EventId,
            TotalAmount = b.TotalAmount,
            ExpiresAt = b.ExpiresAt,

            Details = b.BookingDetails.Select(d => new BookingDetails
            {
                EventSeatId = d.EventSeatId,
                TicketTypeId = d.TicketTypeId,
                TicketTypeName = d.TicketType.Name ?? null,
                Quantity = d.Quantity,
                Price = d.Price
            }).ToList(),

            SeatHolds = b.SeatHolds.Select(s => new SeatHolds
            {
                EventSeatId = s.EventSeatId,
                TicketTypeId = s.TicketTypeId,
                Quantity = s.Quantity
            }).ToList()
        })
        .FirstOrDefaultAsync();
    }

    public async Task<(IQueryable<Booking>, int TotalCount)> GetListBooking(AdminBookingListRequest req)
    {
        var bookings = _dbset.AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Search))
        {
            bookings = bookings.Where(b => (b.User.Email == req.Search) ||
                                           (b.User.Username == req.Search) ||
                                           (b.Event != null &&
                                            b.Event.Name != null &&
                                            EF.Functions.Like(b.Event.Name ?? "", $"%{req.Search}%") ||
                                            (b.Event != null &&
                                            b.Event.Venue != null &&
                                            EF.Functions.Like(b.Event.Venue.Name ?? "", $"%{req.Search}%"))));
        }

        if (req.Status != null) bookings = bookings.Where(b => b.Status == req.Status);

        if (req.DateFrom != null && req.DateTo != null) bookings = bookings.Where(p => p.CreatedAt > req.DateFrom && p.CreatedAt < req.DateTo);

        var total = await bookings.CountAsync();

        if (req.SortDesc) bookings = bookings.OrderByDescending(b => b.CreatedAt);

        bookings = bookings.Skip((req.Page - 1) * req.PageSize).Take(req.PageSize);

        return (bookings, total);
    }

}
