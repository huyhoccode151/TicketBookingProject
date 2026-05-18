using TicketBookingProject.Server.DTOs;

namespace TicketBookingProject.Server;

public interface ICouponService
{
    Task<Result<PagedResponse<CouponListItemResponse>>> GetAllCoupons(CouponListItemRequest req);
}
