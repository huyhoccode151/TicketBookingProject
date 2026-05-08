using Microsoft.EntityFrameworkCore;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class VenueRepository : BaseRepository<Venue>, IVenueRepository
{
    public VenueRepository(TicketBookingProjectContext db) : base(db)
    {
    }

    public async Task<Venue?> GetVenueById(int id)
    {
        var venue = await _dbset.Include(v => v.Events).FirstOrDefaultAsync(v => v.Id == id);
        return venue;
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

    public async Task<(IQueryable<Venue>, int Total)> GetAllVenueAsync(VenueListRequest request)
    {
        var venues = _dbset.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            venues = venues.Where(v => v.Name != null && v.Name.Contains(request.Search));
        }

        if (!string.IsNullOrWhiteSpace(request.Province))
        {
            venues = venues.Where(v => v.Province != null && v.Province.Contains(request.Province));
        }

        var total = await venues.CountAsync();

        venues = venues.OrderByDescending(v => v.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);

        return (venues, total);
    }

    public async Task<bool> DeleteVenueAsync(int id)
    {
        var venue = await GetVenueById(id);
        if (venue == null) return false;
        _dbset.Remove(venue);
        var result = await _db.SaveChangesAsync();
        return result > 0;
    }

    public async Task<Venue> CreateVenueAsync(Venue venue)
    {
        var createdVenue = await _dbset.AddAsync(venue);
        await _db.SaveChangesAsync();
        return venue;
    }

    public async Task<Venue?> UpdateVenueAsync(Venue venue)
    {
        var existingVenue = await GetVenueById(venue.Id);
        if (existingVenue == null) return null;
        _db.Entry(existingVenue).CurrentValues.SetValues(venue);
        await _db.SaveChangesAsync();
        return existingVenue;
    }

}
