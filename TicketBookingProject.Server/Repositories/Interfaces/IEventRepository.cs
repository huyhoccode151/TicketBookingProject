using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface IEventRepository : IBaseRepository<Event>
{
    Task<PagedResponse<Event>> GetEventsAsync(EventListRequest request);
    Task<Event> CreateEventAsync(Event evt);
    Task AddEventWithPosters(Event evt, List<UploadPosterRequest> posters);
    Task AddTicketTypeWithPoster(Event evt, List<TicketType> ticketTypes);
    Task<bool> DeleteEventAsync(int id);
    Task<Event?> GetEventByIdAsync(int id);
    Task UpdateEventWithTicket(Event evt, List<TicketType> newTicketTypes);
    Task UpdateEventWithPosters(Event evt, List<EventPoster> newPosters, List<PosterMetaDto>? posterMeta);
    Task<List<EventPoster>> GetEventPosterByIdAsync(int id);
    Task<List<SeatHold>> HoldTicketsAsync(HoldTicketsRequest request, int userId);
    Task UpdateHoldStatusAsync(List<SeatHold> sh, BookingResponse booking);
}
