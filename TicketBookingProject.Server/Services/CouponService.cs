using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TicketBookingProject.Server.DTOs;

namespace TicketBookingProject.Server;

public class CouponService : ICouponService
{
    private readonly ICouponRepository _couponRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    public CouponService(ICouponRepository couponRepo, ICurrentUserService currentUser, IMapper mapper)
    {
        _couponRepo = couponRepo;
        _currentUser = currentUser;
        _mapper = mapper;
    }
    public async Task<Result<PagedResponse<CouponListItemResponse>>> GetAllCoupons(CouponListItemRequest req)
    {
        var userId = _currentUser.UserId;

        var roles = _currentUser.Role ?? new List<string>();

        var actionOfAdmin = roles.Contains("admin");

        var actionOfOrganizer = roles.Contains("organizer");

        var (coupons, total) = actionOfAdmin
            ? await _couponRepo.GetAllCoupons(req)
            : actionOfOrganizer
                ? await _couponRepo.GetAllCoupons(req, userId) : (null, 0);

        var pagedResponse = await coupons.ProjectTo<CouponListItemResponse>(_mapper.ConfigurationProvider).ToListAsync();

        var result = new PagedResponse<CouponListItemResponse>(
                pagedResponse,
                req.Page,
                req.PageSize,
                total
            );

        if (coupons != null)
        {
            return Result<PagedResponse<CouponListItemResponse>>.Success(result, "Retrived coupons success!!!");
        }

        return Result<PagedResponse<CouponListItemResponse>>.Failure("No Coupon exists", StatusCodes.Status204NoContent);
    }
}
