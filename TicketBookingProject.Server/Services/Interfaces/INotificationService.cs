namespace TicketBookingProject.Server;

public interface INotificationService
{
    Task SendNotificationToUser(int eventId, string message);
    Task<Result<List<Notification>>> GetNotification();
    Task<Result<bool>> MarkAllNotification();
    Task<Result<bool>> MarkNotification(int id);
}
