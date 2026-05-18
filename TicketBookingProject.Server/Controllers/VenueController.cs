using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketBookingProject.Server;
using TicketBookingProject.Server.Common.Extensions;
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

        [HttpGet("list-venue")]
        [Authorize(Roles = "admin,organizer")]
        public async Task<IActionResult> GetListVenue([FromQuery] VenueListRequest request)
        {
            var venues = await _venueService.ListVenueAsync(request);
            return venues.ToActionResult();
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteVenue(int id)
        {
            var result = await _venueService.DeleteVenueAsync(id);
            return result.ToActionResult();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVenueById(int id)
        {
            var result = await _venueService.GetVenueByIdAsync(id);

            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> CreateVenue([FromBody] CreateVenueRequest request)
        {
            var result = await _venueService.CreateVenueAsync(request);
            return result.ToActionResult();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVenue(int id, [FromBody] UpdateVenueRequest request)
        {
            var result = await _venueService.UpdateVenueAsync(id, request);
            return result.ToActionResult();
        }


    }
}
