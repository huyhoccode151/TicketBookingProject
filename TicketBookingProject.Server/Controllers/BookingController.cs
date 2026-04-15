using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketBookingProject.Server;
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

        [HttpGet]
        public async Task<IActionResult> GetListBooking([FromQuery] AdminBookingListRequest req)
        {
            var bookings = await _bookingService.GetListBooking(req);

            return Ok(ApiResponse<PagedResponse<AdminBookingListItemResponse>>.Ok(bookings, "Load list bookings successfully!!!"));
        }
    }
}
