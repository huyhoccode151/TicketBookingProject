using TicketBookingProject.Server.DTOs;

namespace TicketBookingProject.Server;

public class CouponService : ICouponService
{
    private readonly ICouponRepository _couponRepo;
    private readonly ICurrentUserService _currentUser;
    public CouponService(ICouponRepository couponRepo, ICurrentUserService currentUser)
    {
        _couponRepo = couponRepo;
        _currentUser = currentUser;
    }
    public async Task<Result<CouponListItemResponse>> GetAllCoupons(CouponListItemRequest req)
    {
        var userId = _currentUser.UserId;

        var roles = _currentUser.Role ?? new List<string>();

        var actionOfAdmin = roles.Contains("admin");

        var actionOfOrganizer = roles.Contains("organizer");

        var (coupons, total) = actionOfAdmin
            ? await _couponRepo.GetAllCoupons(req)
            : actionOfOrganizer
                ? await _couponRepo.GetAllCoupons(req, userId) : (null, 0);

        if (coupons != null)
        {
            //return Result<CouponListItemResponse>.Success(coupons, "Retrived coupons success!!!");
        }

        return Result<CouponListItemResponse>.Failure("No Coupon exists", StatusCodes.Status204NoContent);
    }
}
