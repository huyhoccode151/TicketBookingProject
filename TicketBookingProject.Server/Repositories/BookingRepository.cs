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
                ExpiresAt = holdResult.First().ExpiresAt?.AddHours(2),
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

            booking.TotalAmount = (long)(booking.BookingDetails.Sum(d => d.Price * d.Quantity) * 1.02);

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

    public async Task<BookingTicketDetails?> GetMyBookingPending(int currentUserId)
    {
        return await _dbset
        .AsNoTracking()
        .Where(b => b.UserId == currentUserId && b.Status == BookingStatus.Pending).OrderByDescending(b => b.CreatedAt)
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

    public async Task<(IQueryable<Booking>, int TotalCount)> GetListBooking(AdminBookingListRequest req, int? organizerId = null)
    {
        var bookings = _dbset.AsQueryable();

        if (organizerId != null) bookings = bookings.Where(b => b.Event.OrganizerId == organizerId);

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

        if (!string.IsNullOrWhiteSpace(req.EventName)) bookings = bookings.Where(b => b.Event.Name == req.EventName);

        if (req.Status != null) bookings = bookings.Where(b => b.Status == req.Status);

        if (req.DateFrom != null && req.DateTo != null) bookings = bookings.Where(p => p.CreatedAt > req.DateFrom && p.CreatedAt < req.DateTo);

        var total = await bookings.CountAsync();

        if (req.SortDesc) bookings = bookings.OrderByDescending(b => b.CreatedAt);

        bookings = bookings.Skip((req.Page - 1) * req.PageSize).Take(req.PageSize);

        return (bookings, total);
    }

    public IQueryable<Booking> GetListRecentBooking(RecentBookingListRequest req)
    {
        var bookings = _dbset.AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.UserName)) bookings = bookings.Where(b => b.Event.Organizer.Username == req.UserName);

        if (!string.IsNullOrWhiteSpace(req.EventName)) bookings = bookings.Where(b => b.Event.Name == req.EventName);

        if (req.DateFrom != null && req.DateTo != null) bookings = bookings.Where(p => p.CreatedAt > req.DateFrom && p.CreatedAt < req.DateTo);

        return bookings.OrderByDescending(b => b.CreatedAt).Take(5);
    }

    public BookingEmailResponseById? GetBookingEmailResponseById(int id)
    {
        var booking = _dbset.Where(b => b.Id == id).Select(e => new BookingEmailResponseById
        {
            Id = e.Id,
            UserEmail = e.User.Email ?? "tieukhuynhtu@gmail.com",
            CustomerName = e.User.Firstname + e.User.Lastname
        }).FirstOrDefault();

        return booking ?? new BookingEmailResponseById();
    }

    public Booking GetBooking(int id) { 
        var booking = _dbset.Where(b => b.Id == id).Select(b => new Booking
        {
            Id = b.Id,
            Status = b.Status,
            TotalAmount = b.TotalAmount,
        }).FirstOrDefault();

        return booking ?? new Booking();
    }

    public async Task<bool> UpdateBookingStatus(int id, BookingStatus status)
    {
        var booking = await _db.Bookings.FindAsync(id);
        if (booking == null) return false;

        booking.Status = status;

        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<bool> RegainQuantityTicketType(int id)
    {
        var booking = await _dbset.Include(b => b.SeatHolds).ThenInclude(s => s.TicketType).FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null) return false;

        if (booking.Status == BookingStatus.Cancelled)
        {
            foreach (var seatHold in booking.SeatHolds)
            {
                if (seatHold.TicketType != null)
                {
                    seatHold.Status = SeatHoldStatus.Released;
                    seatHold.TicketType.SoldQuantity -= seatHold.Quantity;
                    seatHold.TicketType.Quantity += seatHold.Quantity;
                }
            }

            await _db.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> DeleteBooking(int id)
    {
        var booking = _dbset.FirstOrDefault(b => b.Id == id);
        if (booking == null) return false;
        _dbset.Remove(booking);
        await _db.SaveChangesAsync();
        return true;
    }

    //dictionary for booking id and payment id 1 by 1
    public async Task<List<RequestRefundRequest>> CancelBooking(int id, string reason)
    {
        var booking = await _dbset.Include(b => b.Payments).Where(b => b.Event.Id == id && b.Status == BookingStatus.Confirmed).ToListAsync();

        if (!booking.Any()) return new List<RequestRefundRequest>();

        booking.ForEach(b => b.Status = BookingStatus.Cancelled);

        var refunds = booking.Select(b => new RequestRefundRequest
        {
            BookingId = b.Id,
            PaymentId = b.Payments.Where(p => p.Status == PaymentStatus.Success).Select(p => p.Id).FirstOrDefault(),
            Reason = reason,
            Amount = b.TotalAmount
        }).ToList();

        return refunds;
    }
}
