using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketBookingProject.Server;
using TicketBookingProject.Server.Models;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenueController : ControllerBase
    {
        private readonly IVenueService _venueService;
        public VenueController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        [HttpGet("names")]
        public async Task<IActionResult> GetListName(string? req) 
        { 
            var venueNames = await _venueService.ListVenueName(req);
            return Ok(ApiResponse<List<string>>.Ok(venueNames));
        }

        [HttpGet]
        public async Task<IActionResult> GetListVenue()
        {
            var venues = await _venueService.ListVenue();
            return Ok(ApiResponse<List<VenueListItemResponse>>.Ok(venues));
        }
    }
}
