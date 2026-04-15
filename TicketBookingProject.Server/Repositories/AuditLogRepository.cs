using Microsoft.EntityFrameworkCore;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class AuditLogRepository : BaseRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(TicketBookingProjectContext db) : base(db)
    {
    }

    public async Task<PagedResponse<AuditLogDto>> GetListAuditLog(AuditLogRequest req)
    {
        var query = _db.AuditLogs.Include(a => a.User).AsQueryable();
        if (!string.IsNullOrWhiteSpace(req.Search))
            query = query.Where(x => x.User != null &&
                                 x.User.Username.Contains(req.Search));

        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(x => x.CreatedAt)
                               .Skip((req.Page - 1) * req.PageSize)
                               .Take(req.PageSize)
                               .Select(x => new AuditLogDto
                               {
                                   Id = x.Id,
                                   Action = x.Action,
                                   EntityType = x.EntityType,
                                   EntityId = x.EntityId,
                                   Description = x.Description,
                                   CreatedAt = x.CreatedAt,

                                   User = x.User != null
                                        ? new AuditUserDto
                                        {
                                            Username = x.User.Username,
                                            Email = x.User.Email
                                        }
                                        : null
                               })
                               .ToListAsync();
        return new PagedResponse<AuditLogDto>(
            items,
            req.Page,
            req.PageSize,
            totalCount);
    }
}
