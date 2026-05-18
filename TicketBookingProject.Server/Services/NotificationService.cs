using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using TicketBookingProject.Server.Enums;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly TicketBookingProjectContext _db;
    private readonly ICurrentUserService _currentUser;
    public NotificationService(IHubContext<NotificationHub> hubContext, TicketBookingProjectContext db, ICurrentUserService currentUser)
    {
        _hubContext = hubContext;
        _db = db;
        _currentUser = currentUser;
    }

    public async Task SendNotificationToUser(int eventId, string message)
    {
        var userIds = _db.EventSubscriptions
            .Where(es => es.EventId == eventId)
            .Select(es => es.UserId)
            .ToList();

        var now = DateTime.UtcNow;

        foreach (var userId in userIds)
        {
            _db.Notifications.Add(new Notification
            {
                UserId = userId,
                Content = message,
                Status = 1,
                SentAt = now,
                CreatedAt = now
            });

            await _hubContext.Clients.Group($"user-{userId}").SendAsync("ReceiveNotification", message);
        }
        await _db.SaveChangesAsync();
    }

    public async Task<Result<List<Notification>>> GetNotification()
    {
        var userId = _currentUser.UserId;

        var notifications = await _db.Notifications.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAt).Take(20).ToListAsync();
        if (notifications == null || notifications.Count == 0)
        {
            return Result<List<Notification>>.Failure("No notifications found.");
        }

        return Result<List<Notification>>.Success(notifications, "Notifications retrieved successfully.");
    }

    public async Task<Result<bool>> MarkAllNotification()
    {
        var userId = _currentUser.UserId;

        var notifications = await _db.Notifications.Where(n => n.UserId == userId && n.Status == 1)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(n => n.Status, 3));
        if (notifications == 0) return Result<bool>.Failure("Nothing be changed");

        return Result<bool>.Success(true, "Mark all noti success");
    }

    public async Task<Result<bool>> MarkNotification(int id)
    {
        var userId = _currentUser.UserId;
        var notification = await _db.Notifications.Where(n => n.UserId == userId && n.Status == 1 && n.Id == id).FirstOrDefaultAsync();
        if (notification == null) return Result<bool>.Failure("Nothing be changed");

        notification.Status = 3;
        await _db.SaveChangesAsync();

        return Result<bool>.Success(true, "This noti just be readed");
    }
}
