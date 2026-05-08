using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class VnpayRefundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<VnpayRefundService> _logger;

    public VnpayRefundService(IServiceProvider serviceProvider, ILogger<VnpayRefundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TicketBookingProjectContext>();
                var vnPayService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

                // 1. Lấy 20 đơn đang chờ hoàn tiền
                var pendingRefunds = await dbContext.Refunds
                    .Include(r => r.Payment)
                    .Where(p => p.Status == RefundStatus.Pending)
                    .Take(20)
                    .ToListAsync();

                foreach (var refund in pendingRefunds)
                {
                    try
                    {
                        var result = await vnPayService.ExecuteRefundAsync(refund.Payment);
                        // 2. Ghi AuditLog
                        var log = new AuditLog
                        {
                            UserId = refund.Payment.UserId,
                            Action = "VNPAY_REFUND",
                            EntityType = "Payment",
                            EntityId = refund.Payment.Id,
                            Description = $"Refund for booking {refund.Payment.BookingId}. Error Code VNPay: {result?.vnp_ResponseCode}",
                            Metadata = JsonSerializer.Serialize(result),
                            CreatedAt = DateTime.UtcNow
                        };
                        dbContext.AuditLogs.Add(log);

                        // 3. Cập nhật trạng thái Payment
                        if (result?.vnp_ResponseCode == "00")
                        {
                            refund.Status = RefundStatus.Completed;
                            refund.UpdatedAt = DateTime.Now;

                            refund.Payment.Status = PaymentStatus.Refunded;
                        }
                        else
                        {
                            refund.Status = RefundStatus.Rejected;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Lỗi xử lý hoàn tiền ID: {refund.Payment.Id}");
                    }
                }

                await dbContext.SaveChangesAsync();
            }

            // Chờ 5 phút
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
