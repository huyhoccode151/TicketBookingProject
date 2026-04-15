using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketBookingProject.Server;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLog;
        public AuditLogController(IAuditLogService auditLog) => _auditLog = auditLog;

        [HttpGet]
        public async Task<IActionResult> GetListAuditLog([FromQuery] AuditLogRequest req)
        {
            var logs = await _auditLog.GetListAuditLog(req);
            return Ok(ApiResponse<PagedResponse<AuditLogDto>>.Ok(logs));
        }
    }
}
