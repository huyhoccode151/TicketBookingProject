using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface IVenueRepository : IBaseRepository<Venue>
{
    Task<Venue?> GetVenueById(int id);
    Task<List<string>> GetVenueName(string? req);
    Task<List<Venue>> GetAllVenue();
    Task<(IQueryable<Venue>, int Total)> GetAllVenueAsync(VenueListRequest request);
    Task<bool> DeleteVenueAsync(int id);
    Task<Venue> CreateVenueAsync(Venue venue);
    Task<Venue?> UpdateVenueAsync(Venue venue);
}
