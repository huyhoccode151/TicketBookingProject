using static TicketBookingProject.Server.TicketService;

namespace TicketBookingProject.Server;

public interface ITicketService
{
    Task<List<TicketDetailResponse>> CreateTickets(int bookingId);
    Task<List<TicketDetailResponse>> GetTicketsByBookingId(int bookingId);
    Task<PagedResponse<BookingTicketListItemResponse>> GetTicketsByUserId(TicketListRequest req);
    Task<CheckInResult> CheckInAsync(string qrCode);

}
