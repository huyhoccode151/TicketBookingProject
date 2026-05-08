using Microsoft.Identity.Client;

namespace TicketBookingProject.Server;

public class EventUpdateStatusService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventUpdateStatusService> _logger;

    public EventUpdateStatusService(
        IServiceProvider serviceProvider,
        ILogger<EventUpdateStatusService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
                await eventService.UpdateEventStatusAuto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating event statuses");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
