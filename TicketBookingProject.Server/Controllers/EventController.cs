using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata.Ecma335;
using static System.Net.WebRequestMethods;

namespace TicketBookingProject.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        public EventController(IEventService eventService) => _eventService = eventService;


        [HttpGet]
        public async Task<IActionResult> ListAllEvents([FromQuery] EventListRequest req)
        {
            var pagedEvents = await _eventService.ListEventAsync(req);
            if (pagedEvents == null) return NotFound(ApiResponse<PagedResponse<EventListItemResponse>>.Fail("Could found any events!!!"));
            return Ok(ApiResponse<PagedResponse<EventListItemResponse>>.Ok(pagedEvents, "List Users Successfully!!!"));
        }

        [HttpPost]
        public async Task<IActionResult> CreateEventAsync([FromForm] CreateEventRequest request)
        {
            var eventCreated = await _eventService.CreateEventAsync(request);

            if (eventCreated == null) return NoContent();

            return Ok(ApiResponse<EventDetailResponse>.Ok(eventCreated));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventByIdAsync(int id)
        {
            var eventDetail = await _eventService.GetEventByIdAsync(id);
            if (eventDetail == null) return NotFound(ApiResponse<EventDetailResponse>.Fail("Could not found event with this id!!!"));
            return Ok(ApiResponse<EventDetailResponse>.Ok(eventDetail, "Get event successfully!!!"));
        }

        [HttpGet("{id}/poster")]
        public async Task<IActionResult> GetEventPosterByIdAsync(int id)
        {
            var eventPoster = await _eventService.GetEventPosterByIdAsync(id);
            if (eventPoster == null) return NotFound(ApiResponse<List<EventPosterResponse>>.Fail("Could not found event poster with this id!!!"));
            return Ok(ApiResponse<List<EventPosterResponse>>.Ok(eventPoster, "Get event poster successfully!!!"));
        }

        [HttpGet("{id}/ticket-types")]
        public async Task<IActionResult> GetEventTicketTypesByIdAsync(int id)
        {
            var ticketTypes = await _eventService.GetEventTicketTypesByIdAsync(id);
            if (ticketTypes == null) return NotFound(ApiResponse<List<TicketTypeResponse>>.Fail("Could not found event ticket types with this id!!!"));
            return Ok(ApiResponse<List<TicketTypeResponse>>.Ok(ticketTypes, "Get event ticket types successfully!!!"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEventAsync(int id, [FromForm] UpdateEventRequest request)
        {
            var eventUpdated = await _eventService.UpdateEventAsync(id, request);
            if (eventUpdated == null) return NotFound(ApiResponse<EventDetailResponse>.Fail("Could not found event with this id to update!!!"));
            return Ok(ApiResponse<EventDetailResponse>.Ok(eventUpdated, "Update event successfully!!!"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventAsync(int id)
        {
            var isDeleted = await _eventService.DeleteEventAsync(id);
            if (!isDeleted) return NotFound(ApiResponse<bool>.Fail("Could not found event to delete!!!"));
            return Ok(ApiResponse<bool>.Ok(true, "Delete event successfully!!!"));
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0) return BadRequest("No files uploaded");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var maxFileSize = 10 * 1024 * 1024;

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var result = new List<object>();

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                if (file.Length > maxFileSize)
                    return BadRequest($"File {file.FileName} vượt quá 10MB");

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(ext)) return BadRequest($"File {file.FileName} không hợp lệ");

                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                result.Add(new
                {
                    originalName = file.FileName,
                    fileName,
                    url = $"/uploads/{fileName}"
                });

            }
            return Ok(result);
        }

        [HttpPost("hold")]
        public async Task<IActionResult> HoldTickets([FromBody] HoldTicketsRequest request)
        {
            var result = await _eventService.HoldTicketsAsync(request);
            if (result == null) return BadRequest(ApiResponse<bool>.Fail("Could not hold tickets!!!"));
            return Ok(ApiResponse<bool>.Ok(true, "Hold tickets successfully!!!"));
        }
    }
}
