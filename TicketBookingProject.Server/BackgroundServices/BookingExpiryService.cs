using Microsoft.EntityFrameworkCore;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server.BackgroundServices;

public class BookingExpiryService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BookingExpiryService> _logger;
    public BookingExpiryService(IServiceProvider serviceProvider, ILogger<BookingExpiryService> logger)
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
                var context = scope.ServiceProvider.GetRequiredService<TicketBookingProjectContext>();

                await context.Bookings
                    .Where(b => b.Status == BookingStatus.Pending && (b.ExpiresAt < DateTime.UtcNow || b.ExpiresAt == null))
                    .ExecuteUpdateAsync(s => s.SetProperty(b => b.Status, BookingStatus.Expired),
                        stoppingToken);

                _logger.LogInformation("Booking expiry check completed at {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during booking expiry check");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
