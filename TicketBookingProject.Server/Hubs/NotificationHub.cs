using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TicketBookingProject.Server;

[Authorize]
public class NotificationHub : Hub
{
    private readonly CurrentUserService _currentUser;
    public NotificationHub (CurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    public override async Task OnConnectedAsync()
    {
        //var userId = _currentUser.UserId;
        //if (userId.HasValue)
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        //}
        //await base.OnConnectedAsync();
        try
        {
            var userId = _currentUser.UserId;

            if (userId.HasValue)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId.Value}");
            }
            else
            {
                // Xem claim nào đang có trong token
                var claims = Context.User?.Claims
                    .Select(c => $"{c.Type}={c.Value}");
                Console.WriteLine($"[Hub] No userId. Claims: {string.Join(", ", claims ?? [])}");
            }

            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Hub] OnConnectedAsync error: {ex.Message}");
            throw;
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = _currentUser.UserId;

        if (userId.HasValue)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId.Value}");
        }

        await base.OnDisconnectedAsync(exception);
    }
}
