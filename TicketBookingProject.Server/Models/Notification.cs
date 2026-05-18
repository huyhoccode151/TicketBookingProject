using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public byte Type { get; set; }       // 1=email, 2=push, 3=sms
    public string? Title { get; set; }
    public string? Content { get; set; }
    public byte Status { get; set; } = 0; // 0=pending,1=sent,2=failed,3=read
    public DateTime? SentAt { get; set; }
    public DateTime? CreatedAt { get; set; }

    public User User { get; set; } = null!;
}
