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

    public async Task<Result<VenueDetailResponse>> GetVenueByIdAsync(int id)
    {
        var venue = await _venueRepo.GetVenueById(id);
        if (venue == null) return Result<VenueDetailResponse>.Failure("Venue not found", StatusCodes.Status404NotFound);

        return Result<VenueDetailResponse>.Success(_mapper.Map<VenueDetailResponse>(venue), "Show detail venues");
    }

    public async Task<Result<PagedResponse<VenueListItemResponse>>> ListVenueAsync(VenueListRequest request)
    {
        var (venues, total) = await _venueRepo.GetAllVenueAsync(request);

        if (venues == null) return Result<PagedResponse<VenueListItemResponse>>.Failure("No venues found", StatusCodes.Status204NoContent);
        
        var mappedVenues = _mapper.Map<List<VenueListItemResponse>>(venues);
        var pagedResponse = new PagedResponse<VenueListItemResponse>(mappedVenues, request.Page, request.PageSize, total);
        return Result<PagedResponse<VenueListItemResponse>>.Success(pagedResponse, "Venues retrieved successfully");
    }

    public async Task<Result<VenueDetailResponse>> CreateVenueAsync(CreateVenueRequest request)
    {
        var venue = _mapper.Map<Venue>(request);

        var createdVenue = await _venueRepo.CreateVenueAsync(venue);

        if (createdVenue == null) return Result<VenueDetailResponse>.Failure("Failed to create venue", StatusCodes.Status500InternalServerError);

        var response = _mapper.Map<VenueDetailResponse>(createdVenue);
        return Result<VenueDetailResponse>.Success(response, "Venue created successfully");
    }

    public async Task<Result<VenueDetailResponse>> UpdateVenueAsync(int id, UpdateVenueRequest request)
    {
        var existingVenue = await _venueRepo.GetVenueById(id);
        if (existingVenue == null) return Result<VenueDetailResponse>.Failure("Venue not found", StatusCodes.Status404NotFound);
        _mapper.Map(request, existingVenue);
        var updatedVenue = await _venueRepo.UpdateVenueAsync(existingVenue);
        if (updatedVenue == null) return Result<VenueDetailResponse>.Failure("Failed to update venue", StatusCodes.Status500InternalServerError);
        var response = _mapper.Map<VenueDetailResponse>(updatedVenue);
        return Result<VenueDetailResponse>.Success(response, "Venue updated successfully");
    }

    public async Task<Result<bool>> DeleteVenueAsync(int id)
    {
        var venue = await _venueRepo.GetVenueById(id);

        if (venue == null) return Result<bool>.Failure("Venue not found", StatusCodes.Status404NotFound);
        else if (venue.Events != null && venue.Events.Any()) 
            return Result<bool>.Failure("Cannot delete venue with associated events", StatusCodes.Status400BadRequest);

        var deleted = await _venueRepo.DeleteVenueAsync(id);
        if (!deleted) return Result<bool>.Failure("Failed to delete venue", StatusCodes.Status500InternalServerError);

        return Result<bool>.Success(true, "Venue deleted successfully");
    }
}   
