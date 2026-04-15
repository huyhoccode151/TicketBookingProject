namespace TicketBookingProject.Server;

public interface IAuditLogService
{
    void AddLog(string action, string entityType, long? entityId, string description, object? metadata = null, int? userId = null);
    Task<PagedResponse<AuditLogDto>> GetListAuditLog(AuditLogRequest req);
}
