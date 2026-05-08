using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TicketBookingProject.Server;
using TicketBookingProject.Server.Common.Extensions;
using TicketBookingProject.Server.Models;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IBookingService _bookingService;

        public BookingController(IEventService eventService, IBookingService bookingService)
        {
            _eventService = eventService;
            _bookingService = bookingService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBookingAsync([FromBody] HoldTicketsRequest request)
        {
            var holdResult = await _eventService.HoldTicketsAsync(request);

            var bookingResult = await _bookingService.CreateBookingAsync(holdResult);

            await _eventService.UpdateHoldStatusAsync(holdResult, bookingResult);

            return Ok(ApiResponse<BookingResponse>.Ok(bookingResult, "Create booking successfully!!!"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingByUserIdAsync(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);

            return Ok(ApiResponse<BookingTicketDetails?>.Ok(booking, "Get booking successfully!!!"));
        }

        [HttpGet("email-booking/{id}")]
        public async Task<IActionResult> GetBookingEmailResponseById(int id)
        {
            var booking = await _bookingService.GetBookingEmailResponseById(id);

            return Ok(ApiResponse<BookingEmailResponseById?>.Ok(booking, "Get booking successfully!!!"));
        }

        
        [HttpGet]
        [Authorize(Roles = "admin,organizer")]
        [HasPermission("booking:manage")]
        public async Task<IActionResult> GetListBooking([FromQuery] AdminBookingListRequest req)
        {
            var bookings = await _bookingService.GetListBooking(req);

            return bookings.ToActionResult();
        }

        [HttpGet("recent-booking")]
        [Authorize(Roles = "admin,organizer")]
        [HasPermission("booking:manage")]
        public async Task<IActionResult> GetListRecentBooking([FromQuery] RecentBookingListRequest req)
        {
            var bookings = await _bookingService.GetListRecentBooking(req);

            return bookings.ToActionResult();
        }

        [HttpDelete("{bookingId}")]
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {
            var booking = await _bookingService.DeleteBooking(bookingId);
            return Ok(ApiResponse<bool>.Ok(booking, "Delete Booking Success!!!"));
        }
    }
}
