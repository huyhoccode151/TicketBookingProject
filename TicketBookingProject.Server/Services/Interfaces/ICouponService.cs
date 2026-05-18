using TicketBookingProject.Server.DTOs;

namespace TicketBookingProject.Server;

public interface ICouponService
{
    Task<Result<CouponListItemResponse>> GetAllCoupons(CouponListItemRequest req);
}
