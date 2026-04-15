using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class AuditLog : IEntities
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string Action { get; set; } = null!;
    public string EntityType { get; set; } = null!;
    public long? EntityId { get; set; }
    public string Description { get; set; } = null!;
    public string? Metadata { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public User? User { get; set; }
}
