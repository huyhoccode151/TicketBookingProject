namespace TicketBookingProject.Server;

public interface IAuditLogRepository : IBaseRepository<AuditLog>
{
    Task<PagedResponse<AuditLogDto>> GetListAuditLog(AuditLogRequest req);
    Task<List<AuditLogDto>> GetListAuditLogsByUserId(int userId);
}
