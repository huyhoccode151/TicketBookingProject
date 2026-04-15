using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface IBookingService
{
    Task<BookingResponse> CreateBookingAsync(List<SeatHold> holdResult);
    Task<BookingTicketDetails?> GetBookingByIdAsync(int id);
    Task<PagedResponse<AdminBookingListItemResponse>> GetListBooking(AdminBookingListRequest req);
}
