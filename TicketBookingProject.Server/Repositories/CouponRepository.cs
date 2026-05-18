using TicketBookingProject.Server.DTOs;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class CouponRepository : BaseRepository<Coupon>, ICouponRepository
{
    public CouponRepository(TicketBookingProjectContext db) : base(db)
    {

    }

    public async Task<(IQueryable<Coupon>, int Total)> GetAllCoupons(CouponListItemRequest req, int? userId = null)
    {
        var coupons = _dbset.AsQueryable();

        if (userId != null) coupons = coupons.Where(c => c.CreatedBy == userId);

        var total = 0;

        return (coupons, total);
    }
}
