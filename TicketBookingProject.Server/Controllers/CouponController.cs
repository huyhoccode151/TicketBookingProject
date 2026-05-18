using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TicketBookingProject.Server;
using TicketBookingProject.Server.DTOs;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        //[HttpGet]
        //[Authorize(Roles = "admin,organizer")]
        //public Task<IActionResult> GetAllCoupons([FromQuery] CouponListItemRequest request)
        //{
        //    var coupons = _couponService.GetAllCoupons(request);

        //    //return coupons.ToActionResult();
        //    return Ok();
        //}
    }
}