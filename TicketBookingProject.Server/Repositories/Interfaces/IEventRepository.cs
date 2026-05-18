using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface IEventRepository : IBaseRepository<Event>
{
    Task<(IQueryable<Event>, int TotalCount)> GetEventsAsync(EventListRequest req, bool published = false, int? organizerId = null);
    Task<IQueryable<Event>> GetFavEvent(int currentUserId);
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
    Task<List<EventTrendingResponse>> GetEventTrending();
    Task<List<string>> GetEventName(string? req);
    Task<List<(Event Event, string oStatus, string nStatus)>> UpdateEventStatusAuto();
    Task<IQueryable<Event>> GetRelatedEvents(int id, int categoryId, int? numTake = null);
}
