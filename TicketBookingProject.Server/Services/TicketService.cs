using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepo;
    private readonly IBookingRepository _bookingRepo;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public TicketService(ITicketRepository ticketRepo, IBookingRepository bookingRepo, IMapper mapper, ICurrentUserService currentUser)
    {
        _ticketRepo = ticketRepo;
        _bookingRepo = bookingRepo;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<List<TicketDetailResponse>> CreateTickets(int bookingId)
    {
        var booking = await _bookingRepo.GetBookingByIdAsync(bookingId);

        if (booking is not null)
        {
            var tickets = await _ticketRepo.CreateTickets(booking);
            
            return _mapper.Map<List<TicketDetailResponse>>(tickets);
        }

        return new List<TicketDetailResponse>();
    }

    public async Task<List<TicketDetailResponse>> GetTicketsByBookingId(int bookingId)
    {
        var tickets = await _ticketRepo.GetTicketsByBookingId(bookingId);

        return _mapper.Map<List<TicketDetailResponse>>(tickets);
    }

    public async Task<PagedResponse<BookingTicketListItemResponse>> GetTicketsByUserId(TicketListRequest req)
    {
        var userId = _currentUser.UserId;
        if (userId == null) return new PagedResponse<BookingTicketListItemResponse>(
                                    new List<BookingTicketListItemResponse>(),
                                    req.Page,
                                    req.PageSize,
                                    0
                                   );
        else
        {
            var (bookings, total) = await _ticketRepo.GetTicketsByUserId(userId.Value, req);

            var pagedBookings = await bookings
                .ProjectTo<BookingTicketListItemResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResponse<BookingTicketListItemResponse>(
                    pagedBookings,
                    req.Page,
                    req.PageSize,
                    total
                );
        }
    }
}
