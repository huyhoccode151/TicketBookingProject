using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface IEventService
{
    Task<PagedResponse<EventListItemResponse>> ListEventAsync(EventListRequest request);

    Task<EventDetailResponse> CreateEventAsync(CreateEventRequest request);
    Task<EventDetailResponse?> UpdateEventAsync(int id, UpdateEventRequest request);
    Task<bool> DeleteEventAsync(int id);
    Task<EventDetailResponse?> GetEventByIdAsync(int id);
    Task<List<EventPosterResponse>> GetEventPosterByIdAsync(int id);
    Task<List<TicketTypeResponse>> GetEventTicketTypesByIdAsync(int id);
    Task<List<SeatHold>> HoldTicketsAsync(HoldTicketsRequest request);
    Task UpdateHoldStatusAsync(List<SeatHold> sh, BookingResponse booking);
}
