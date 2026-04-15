using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface IPaymentRepository : IBaseRepository<Payment>
{
    Task<Payment> CreatePaymentAsync(Payment payment);

    Task<(IQueryable<Payment>, int TotalCount)> GetListPayment(AdminPaymentListRequest req);
    Task<bool> DeletePayment(Payment payment);
    Task<Payment> GetPaymentById(int id);
}
