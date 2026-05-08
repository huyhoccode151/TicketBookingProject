using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketBookingProject.Server;
using TicketBookingProject.Server.Common.Extensions;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLog;
        public AuditLogController(IAuditLogService auditLog) => _auditLog = auditLog;

        [HttpGet]
        [Authorize(Roles = "admin")]
        [HasPermission("audit-log:manage")]
        public async Task<IActionResult> GetListAuditLog([FromQuery] AuditLogRequest req)
        {
            var logs = await _auditLog.GetListAuditLog(req);
            return Ok(ApiResponse<PagedResponse<AuditLogDto>>.Ok(logs));
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetListAuditLogByUserId()
        {
            var logs = await _auditLog.GetListAuditLogsByUserId();

            return logs.ToActionResult();
        }
    }
}
