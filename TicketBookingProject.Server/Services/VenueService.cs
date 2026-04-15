using AutoMapper;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class VenueService : IVenueService
{
    private readonly IVenueRepository _venueRepo;
    private readonly IMapper _mapper;
    public VenueService(IVenueRepository venueRepo, IMapper mapper)
    {
        _venueRepo = venueRepo;
        _mapper = mapper;
    }
    public async Task<List<string>> ListVenueName(string? req)
    {
        var venueNames = await _venueRepo.GetVenueName(req);
        return venueNames;
    }

    public async Task<List<VenueListItemResponse>> ListVenue()
    {
        var venues = await _venueRepo.GetAllVenue();
        
        return _mapper.Map<List<VenueListItemResponse>>(venues);
    }
}
