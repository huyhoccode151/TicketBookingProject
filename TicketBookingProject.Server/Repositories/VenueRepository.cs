using Microsoft.EntityFrameworkCore;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class VenueRepository : BaseRepository<Venue>, IVenueRepository
{
    public VenueRepository(TicketBookingProjectContext db) : base(db)
    {
    }

    public async Task<List<string>> GetVenueName(string? req)
    {
        if (string.IsNullOrWhiteSpace(req))
            return await _dbset
            .Select(v => v.Name!)
            .ToListAsync();

        var venueNames = await _dbset
            .Where(v => v.Name != null && v.Name.Contains(req))
            .Select(v => v.Name!)
            .ToListAsync();

        return venueNames;
    }

    public async Task<List<Venue>> GetAllVenue()
    {
        var venues = await _dbset.ToListAsync();

        return venues;
    } 

}
