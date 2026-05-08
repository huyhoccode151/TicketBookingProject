using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface IRefundRepository : IBaseRepository<Refund>
{
    Task<bool> CreateRefund(List<RequestRefundRequest> listRefunds, int? processBy);
}
