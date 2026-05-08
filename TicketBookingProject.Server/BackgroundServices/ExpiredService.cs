using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server.BackgroundServices;

public class ExpiredService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public ExpiredService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

                await eventService.CleanupExpiredHoldsAsync();
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
