using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface IBookingRepository : IBaseRepository<Booking>
{
    Task<BookingResponse> CreateBookingAsync(int userId, List<SeatHold> holdResult);
    Task<BookingTicketDetails?> GetBookingByIdAsync(int id);
    Task<(IQueryable<Booking>, int TotalCount)> GetListBooking(AdminBookingListRequest req);
}
