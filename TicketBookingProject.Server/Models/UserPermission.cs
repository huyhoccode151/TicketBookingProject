using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class UserPermission : IEntities
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public int PermissionId { get; set; }
    public sbyte Effect { get; set; } = 1;
    public DateTime? CreatedAt { get; set; }
    public User User { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
