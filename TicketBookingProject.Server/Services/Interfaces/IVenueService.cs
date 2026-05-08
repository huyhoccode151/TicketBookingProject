namespace TicketBookingProject.Server;

public interface IVenueService
{
    Task<List<string>> ListVenueName(string? req);
    Task<List<VenueListItemResponse>> ListVenue();
    Task<Result<PagedResponse<VenueListItemResponse>>> ListVenueAsync(VenueListRequest request);
    Task<Result<bool>> DeleteVenueAsync(int id);
    Task<Result<VenueDetailResponse>> CreateVenueAsync(CreateVenueRequest request);
    Task<Result<VenueDetailResponse>> UpdateVenueAsync(int id, UpdateVenueRequest request);
    Task<Result<VenueDetailResponse>> GetVenueByIdAsync(int id);
}
