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

    public async Task<Result<TicketDetailResponse>> GetTicketById(int ticketId)
    {
        var ticket = await _ticketRepo.GetTicketByIdAsync(ticketId);
        if (ticket is not null)
            return Result<TicketDetailResponse>.Success(_mapper.Map<TicketDetailResponse>(ticket));
        return Result<TicketDetailResponse>.Failure("Could not found ticket with this id!!!");
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

    public async Task<Result<List<BookingTicketListItemResponse>>> GetUpcomingTicketsByUserId()
    {
        var userId = _currentUser.UserId ?? 0;

        var bookings = await _ticketRepo.GetUpcomingTicketsByUserId(userId);
        var result = await bookings.ProjectTo<BookingTicketListItemResponse>(_mapper.ConfigurationProvider)
            .ToListAsync();

        if (result == null) return Result<List<BookingTicketListItemResponse>>.Failure("Cant retrived upcoming tickets events!!!", StatusCodes.Status204NoContent);

        return Result<List<BookingTicketListItemResponse>>.Success(result, "Retrived upcoming tickets events successfully!!!");
    }

    public async Task<CheckInResult> CheckInAsync(string qrCode)
    {
        var ticket = await _ticketRepo.GetByQrCodeAsync(qrCode);

        if (ticket == null)
            return new CheckInResult(false, "Ticket does not exist.");

        if (ticket.Status == TicketStatus.Used)
            return new CheckInResult(false, "Ticket was used!", MapToDto(ticket));

        if (ticket.Status == TicketStatus.Cancelled)
            return new CheckInResult(false, "Ticket was cancelled!", MapToDto(ticket));

        if (ticket.Status == TicketStatus.Expired)
            return new CheckInResult(false, "Ticket was expired.", MapToDto(ticket));

        ticket.Status = TicketStatus.Used;
        ticket.CheckedInAt = DateTime.UtcNow;
        ticket.CheckedInBy = _currentUser.UserId;

        await _ticketRepo.UpdateTicketAsync(ticket);

        var dto = new CheckInTicketDto
        {
            Id = ticket.Id,
            CustomerName = $"{ticket.Booking?.User?.Firstname} {ticket.Booking?.User?.Lastname}",
            TicketTypeName = ticket.TicketType?.Name,
            SeatNumber = ticket.EventSeat?.Seat.SeatNumber,
            CheckedInAt = ticket.CheckedInAt
        };

        return new CheckInResult(true, "Check-in success", dto);
    }

    private CheckInTicketDto MapToDto(Ticket ticket) => new()
    {
        Id = ticket.Id,
        CustomerName = $"{ticket.Booking?.User?.Firstname} {ticket.Booking?.User?.Lastname}",
        TicketTypeName = ticket.TicketType?.Name,
        SeatNumber = ticket.EventSeat?.Seat.SeatNumber,
        CheckedInAt = ticket.CheckedInAt
    };
}
