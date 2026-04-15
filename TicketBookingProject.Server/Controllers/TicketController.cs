using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketBookingProject.Server;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetTicketByBookingId(int bookingId)
        {
            var tickets = await _ticketService.GetTicketsByBookingId(bookingId);

            return Ok(ApiResponse<List<TicketDetailResponse>>.Ok(tickets, "Load tickets successfully!!!"));
        }

        [Authorize]
        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetTicketByUserId([FromQuery] TicketListRequest req)
        {
            var tickets = await _ticketService.GetTicketsByUserId(req);

            return Ok(ApiResponse<PagedResponse<BookingTicketListItemResponse>>.Ok(tickets, "Load tickets successfully!!!"));
        }
    }
}
