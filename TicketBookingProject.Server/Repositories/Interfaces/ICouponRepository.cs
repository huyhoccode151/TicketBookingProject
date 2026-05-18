using TicketBookingProject.Server.DTOs;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface ICouponRepository
{
    Task<(IQueryable<Coupon>, int Total)> GetAllCoupons(CouponListItemRequest req, int? userId = null);
}
