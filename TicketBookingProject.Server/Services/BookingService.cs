using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using TicketBookingProject.Server.Enums;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLog;
    public BookingService(IBookingRepository bookingRepo, IMapper mapper, ICurrentUserService currentUser, IAuditLogService auditLog)
    {
        _bookingRepo = bookingRepo;
        _mapper = mapper;
        _currentUser = currentUser;
        _auditLog = auditLog;
    }

    public async Task<BookingResponse> CreateBookingAsync(List<SeatHold> holdResult)
    {
        var userId = (int)_currentUser.UserId!;
        var booking = await _bookingRepo.CreateBookingAsync(userId, holdResult);

        _auditLog.AddLog(
           AuditAction.BookingCreated,
           "Booking",
           userId,
           $"Account {userId} just created a booking",
           new
           {
               userId,
               booking,
           },
           userId
           );
        return booking;
    }

    public async Task<BookingTicketDetails?> GetBookingByIdAsync(int id)
    {
        var bookings = await _bookingRepo.GetBookingByIdAsync(id);

        return bookings;
    }

    public async Task<Result<BookingTicketDetails>> GetMyBookingPending()
    {
        var currentUserId = _currentUser.UserId ?? 0;
        var booking = await _bookingRepo.GetMyBookingPending(currentUserId);
        if (booking == null) return Result<BookingTicketDetails>.Success(new BookingTicketDetails(), "Has no Booking Detail");

        return Result<BookingTicketDetails>.Success(booking, "Retrived booking pending!!!");
    }

    public async Task<Result<PagedResponse<AdminBookingListItemResponse>>> GetListBooking(AdminBookingListRequest req)
    {
        var currentOrganizerId = _currentUser.UserId;

        var roles = _currentUser.Role ?? new List<string>();

        var actionOfAdmin = roles.Contains("admin");

        var actionOfOrganizer = roles.Contains("organizer");

        var (bookings, total) = actionOfAdmin
            ? await _bookingRepo.GetListBooking(req)
            : actionOfOrganizer
                ? await _bookingRepo.GetListBooking(req, currentOrganizerId)
                : (null, 0);

        if (bookings == null)
        {
            return Result<PagedResponse<AdminBookingListItemResponse>>
                .Failure("Get List Booking Failed!!!", StatusCodes.Status203NonAuthoritative);
        }

        var pagedBooking = await bookings
                .ProjectTo<AdminBookingListItemResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

        var result = new PagedResponse<AdminBookingListItemResponse>(
                pagedBooking,
                req.Page,
                req.PageSize,
                total
            );

        return Result<PagedResponse<AdminBookingListItemResponse>>.Success(result, "Get List Booking Successfully");
    }

    public async Task<Result<List<AdminBookingListItemResponse>>> GetListRecentBooking(RecentBookingListRequest req)
    {
        var bookings = _bookingRepo.GetListRecentBooking(req);

        var currentOrganizerId = _currentUser.UserId;

        var actionOfAdmin = _currentUser.Role!.Contains("admin");

        var actionOfOrganizer = _currentUser.Role!.Contains("organizer");

        if (actionOfAdmin)
        {
            var listRecentBooking = await bookings.ProjectTo<AdminBookingListItemResponse>(_mapper.ConfigurationProvider).ToListAsync();
            return Result<List<AdminBookingListItemResponse>>.Success(listRecentBooking, "Load admin booking item success");
        }
        else if (!actionOfAdmin && actionOfOrganizer)
        {
            var listRecentBooking = await bookings.Where(b => b.Event.OrganizerId == currentOrganizerId).ProjectTo<AdminBookingListItemResponse>(_mapper.ConfigurationProvider).ToListAsync();
            return Result<List<AdminBookingListItemResponse>>.Success(listRecentBooking, "Load organizer booking item success");
        }

        return Result<List<AdminBookingListItemResponse>>.Failure("Get List Booking Failed", StatusCodes.Status203NonAuthoritative);
    }

    public async Task<BookingEmailResponseById?> GetBookingEmailResponseById(int id)
    {
        var booking = _bookingRepo.GetBookingEmailResponseById(id);

        return booking;
    }

    public async Task<bool> UpdateBookingStatus(int id, BookingStatus status)
    {
        var booking = _bookingRepo.GetBooking(id);

        if (booking == null)
            return false;

        if (booking.Status == BookingStatus.Confirmed)
            return false;

        var updated = await _bookingRepo.UpdateBookingStatus(booking.Id, status);

        _auditLog.AddLog(
          status.ToString(),
          "Booking",
          id,
          $"Booking {id} has been {status.ToString()}",
          new
          {
              BookingId = booking.Id,
              OldStatus = booking.Status,
              NewStatus = status,
              TotalAmount = booking.TotalAmount
          }
          );

        return updated;
    } 

    public async Task<bool> RegainQuantityTicketType(int id)
    {
        var booking = await _bookingRepo.RegainQuantityTicketType(id);

        return booking;
    }

    public async Task<bool> DeleteBooking(int id)
    {
        return await _bookingRepo.DeleteBooking(id);
    }
}
