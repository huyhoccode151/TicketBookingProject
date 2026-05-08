using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface IBookingService
{
    Task<BookingResponse> CreateBookingAsync(List<SeatHold> holdResult);
    Task<BookingTicketDetails?> GetBookingByIdAsync(int id);
    Task<Result<PagedResponse<AdminBookingListItemResponse>>> GetListBooking(AdminBookingListRequest req);
    Task<Result<List<AdminBookingListItemResponse>>> GetListRecentBooking(RecentBookingListRequest req);
    Task<BookingEmailResponseById?> GetBookingEmailResponseById(int id);
    Task<bool> UpdateBookingStatus(int id, BookingStatus status);
    Task<bool> RegainQuantityTicketType(int id);
    Task<bool> DeleteBooking(int id);
}
