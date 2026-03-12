namespace TicketBookingProject.Server;

public interface IDelEntity
{
    DateTime? DeletedAt { get; set; }
}
