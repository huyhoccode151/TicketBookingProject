namespace TicketBookingProject.Server;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? UserName { get; }
    List<string>? Role { get; }
    List<string>? Permission {  get; }
}
