using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class EventPosterRepository : BaseRepository<EventPoster>, IEventPosterRepository
{
    public EventPosterRepository(TicketBookingProjectContext db) : base(db)
    {
    }

    //public async Task<List<EventPoster>> AddEventPoster(int id, List<object> request)
    //{
    //    var posters = new List<EventPoster>();
    //    foreach (var poster in request) 
    //    {
    //        posters.Add(new EventPoster
    //        {
    //            EventId = poster.eventId,

    //        });
    //    }
    //}
}
