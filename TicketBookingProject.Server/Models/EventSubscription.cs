using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class EventSubscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int EventId { get; set; }
    public byte Status { get; set; } = 1; // 1=active, 0=inactive
    public DateTime? CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public Event Event { get; set; } = null!;
}
