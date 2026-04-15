namespace TicketBookingProject.Server;

public interface IVenueService
{
    Task<List<string>> ListVenueName(string? req);
    Task<List<VenueListItemResponse>> ListVenue();
}
