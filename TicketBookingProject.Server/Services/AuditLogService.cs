using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class AuditLogService : IAuditLogService
{
    private readonly TicketBookingProjectContext _context;
    private readonly IHttpContextAccessor _http;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogRepository _auditLogRepo;
    public AuditLogService(TicketBookingProjectContext context, IHttpContextAccessor http, ICurrentUserService currentUser, IAuditLogRepository auditLogRepo)
    {
        _context = context;
        _http = http;
        _currentUser = currentUser;
        _auditLogRepo = auditLogRepo;
    }

    public void AddLog(string action, string entityType, long? entityId, string description, object? metadata = null, int? userId = null)
    {
        var log = new AuditLog
        {
            UserId = userId ?? _currentUser.UserId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Description = description,
            Metadata = metadata != null ? System.Text.Json.JsonSerializer.Serialize(metadata) : null,
            CreatedAt = DateTime.UtcNow
        };

        _context.AuditLogs.Add(log);
    }

    public async Task<PagedResponse<AuditLogDto>> GetListAuditLog(AuditLogRequest req)
    {
        var logs = await _auditLogRepo.GetListAuditLog(req);
        return logs;
    }
}
