using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class RefundRepository : BaseRepository<Refund>,IRefundRepository
{
    public RefundRepository(TicketBookingProjectContext db) : base(db)
    {
    
    }

    public async Task<bool> CreateRefund(List<RequestRefundRequest> listRefunds, int? processBy)
    {
        var processAt = DateTime.UtcNow;
        var createAt = DateTime.UtcNow;
        var updateAt = DateTime.UtcNow;

        var refunds = listRefunds.Select(b => new Refund
        {
            BookingId = b.BookingId,
            PaymentId = b.PaymentId,
            Reason = b.Reason,
            Amount = b.Amount,
            Status = RefundStatus.Pending,
            ProcessedBy = processBy,
            ProcessedAt = processAt,
            UpdatedAt = updateAt,
            CreatedAt = createAt,
        }).ToList();

        _dbset.AddRange(refunds);

        return true;
    }
}
