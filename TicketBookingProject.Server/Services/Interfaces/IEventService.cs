using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface IEventService
{
    Task<Result<PagedResponse<EventListItemResponse>>> ListEventAsync(EventListRequest req);
    Task<Result<List<EventListItemResponse>>> GetFavEvent();
    Task<EventDetailResponse> CreateEventAsync(CreateEventRequest request);
    Task<EventDetailResponse?> UpdateEventAsync(int id, UpdateEventRequest request);
    Task<bool> DeleteEventAsync(int id);
    Task<EventDetailResponse?> GetEventByIdAsync(int id);
    Task<List<EventPosterResponse>> GetEventPosterByIdAsync(int id);
    Task<List<TicketTypeResponse>> GetEventTicketTypesByIdAsync(int id);
    Task<List<SeatHold>> HoldTicketsAsync(HoldTicketsRequest request);
    Task UpdateHoldStatusAsync(List<SeatHold> sh, BookingResponse booking);
    Task<List<EventTrendingResponse>> GetEventTrending();
    Task<List<string>> GetEventName(string? req);
    Task CleanupExpiredHoldsAsync();
    Task<Result<EventDetailResponse>> UpdateEventStatusAsync(int id, UpdateEventStatusRequest request);
    Task UpdateEventStatusAuto();
    Task<Result<bool>> DeleteBooking(int id);
    Task<Result<List<RelatedEventResponse>>> GetRelatedEvents(int id, RelatedEventRequest req);
}
