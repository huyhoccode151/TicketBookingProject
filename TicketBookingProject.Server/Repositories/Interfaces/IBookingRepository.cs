using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface IBookingRepository : IBaseRepository<Booking>
{
    Task<BookingResponse> CreateBookingAsync(int userId, List<SeatHold> holdResult);
    Task<BookingTicketDetails?> GetBookingByIdAsync(int id);
    Task<(IQueryable<Booking>, int TotalCount)> GetListBooking(AdminBookingListRequest req, int? organizerId = null);
    IQueryable<Booking> GetListRecentBooking(RecentBookingListRequest req);
    BookingEmailResponseById? GetBookingEmailResponseById(int id);
    Booking GetBooking(int id);
    Task<bool> UpdateBookingStatus(int id, BookingStatus status);
    Task<bool> RegainQuantityTicketType(int id);
    Task<bool> DeleteBooking(int id);
    Task<List<RequestRefundRequest>> CancelBooking(int id, string reason);
    Task<BookingTicketDetails?> GetMyBookingPending(int currentUserId);
}
