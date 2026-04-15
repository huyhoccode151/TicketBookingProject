using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface ITicketRepository : IBaseRepository<Ticket>
{
    Task<List<Ticket>> CreateTickets(BookingTicketDetails booking);
    Task<List<Ticket>> GetTicketsByBookingId(int bookingId);
    Task<(IQueryable<Booking>, int TotalCount)> GetTicketsByUserId(int userId, TicketListRequest req);
}
