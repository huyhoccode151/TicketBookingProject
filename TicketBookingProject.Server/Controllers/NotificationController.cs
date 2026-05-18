using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketBookingProject.Server;
using TicketBookingProject.Server.Common.Extensions;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notiService;
        public NotificationController(INotificationService notiService)
        {
            _notiService = notiService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetNotification()
        {
            var notifications = await _notiService.GetNotification();
            return notifications.ToActionResult();
        }

        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> MarkAllReaded()
        {
            var noti = await _notiService.MarkAllNotification();

            return noti.ToActionResult();
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> MarkAsReaded(int id) {
            var noti = await _notiService.MarkNotification(id);
            return noti.ToActionResult();
        }
    }
}
