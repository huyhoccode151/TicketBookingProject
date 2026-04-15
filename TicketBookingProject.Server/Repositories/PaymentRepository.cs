using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(TicketBookingProjectContext db) : base(db)
    {

    }

    public async Task<Payment> CreatePaymentAsync(Payment payment)
    {
        await _dbset.AddAsync(payment);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("=== ERROR ===");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.InnerException?.Message);
            Console.WriteLine(ex.InnerException?.InnerException?.Message);
            throw;
        }
        return payment;
    }

    public async Task<(IQueryable<Payment>, int TotalCount)> GetListPayment(AdminPaymentListRequest req)
    {
        var payments = _dbset.AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Search))
        {
            payments = payments.Where(p => (p.User.Email != null && p.User.Email == req.Search) ||
                                    (p.User.Username != null && p.User.Username == req.Search) ||
                                    (p.PaymentTransaction != null && p.PaymentTransaction == req.Search) ||
                                    (p.Booking.Event.Name != null &&
                                    EF.Functions.Like(p.Booking.Event.Name ?? "", $"%{req.Search}%")));
        }

        if (req.Status != null) payments = payments.Where(p => (req.Status != null && p.Status == req.Status));

        if (req.Method != null) payments = payments.Where(p => (req.Method != null && p.PaymentMethod == req.Method));

        if (req.DateFrom != null && req.DateTo != null) payments = payments.Where(p => p.CreatedAt > req.DateFrom && p.CreatedAt < req.DateTo);

        var totalCount = await payments.CountAsync();

        if (req.SortDesc) payments = payments.OrderByDescending(x => x.CreatedAt);

        payments = payments
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize);

        return (payments, totalCount);
    }

    public async Task<bool> DeletePayment(Payment payment)
    {
        await this.ForceDeleteAsync(payment);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<Payment> GetPaymentById(int id)
    {
        var payment = _dbset.FirstOrDefault(x => x.Id == id);

        return payment ?? new Payment();
    }
}
