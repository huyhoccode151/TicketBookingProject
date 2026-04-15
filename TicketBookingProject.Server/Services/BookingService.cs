using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    public BookingService(IBookingRepository bookingRepo, IMapper mapper, ICurrentUserService currentUser)
    {
        _bookingRepo = bookingRepo;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<BookingResponse> CreateBookingAsync(List<SeatHold> holdResult)
    {
        var userId = (int)_currentUser.UserId!;
        var booking = await _bookingRepo.CreateBookingAsync(userId, holdResult);

        return booking;
    }

    public async Task<BookingTicketDetails?> GetBookingByIdAsync(int id)
    {
        var bookings = await _bookingRepo.GetBookingByIdAsync(id);

        return bookings;
    }

    public async Task<PagedResponse<AdminBookingListItemResponse>> GetListBooking(AdminBookingListRequest req)
    {
        var (bookings, total) = await _bookingRepo.GetListBooking(req);

        var pagedBookings = await bookings
                .ProjectTo<AdminBookingListItemResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

        return new PagedResponse<AdminBookingListItemResponse>(
                    pagedBookings,
                    req.Page,
                    req.PageSize,
                    total
                );
    }
}
