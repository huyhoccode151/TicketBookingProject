using TicketBookingProject.Server.DTOs;
using TicketBookingProject.Server.Enums;
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

        if (!string.IsNullOrWhiteSpace(req.Search)) coupons = coupons.Where(c => c.Code.Contains(req.Search) );

        if (req.DiscountType != null) coupons = coupons.Where(c => c.DiscountType == req.DiscountType);

        if (req.DateCreateFrom.HasValue && req.DateCreateTo.HasValue) coupons = coupons.Where(c => c.CreatedAt >= req.DateCreateFrom && c.CreatedAt <= req.DateCreateTo);

        if (req.DateExpiredFrom.HasValue && req.DateExpiredTo.HasValue) coupons = coupons.Where(c => c.ExpiredAt >= req.DateExpiredFrom && c.ExpiredAt <= req.DateExpiredTo);

        var total = coupons.Count();

        coupons = coupons.OrderByDescending(c => c.CreatedAt).Skip((req.Page - 1) * req.PageSize).Take(req.PageSize);

        return (coupons, total);
    }
}
