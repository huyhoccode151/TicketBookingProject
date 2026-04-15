using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface IVenueRepository : IBaseRepository<Venue>
{
    Task<List<string>> GetVenueName(string? req);
    Task<List<Venue>> GetAllVenue();
}
