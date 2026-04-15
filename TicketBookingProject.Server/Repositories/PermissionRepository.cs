using Microsoft.EntityFrameworkCore;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
{
    public PermissionRepository(TicketBookingProjectContext db) : base(db)
    {
    }
     
    public async Task<List<Permission>> GetPermissionsByName(List<string> ListNamePermissions)
    {
        var permissions = await _db.Permissions.Where(p => ListNamePermissions.Contains(p.Name)).ToListAsync();
        return permissions;
    }

    public async Task<Permission?> GetPermissionByName(string name)
    {
        var permission = await _db.Permissions.Where(p => p.Name == name).FirstOrDefaultAsync();
        return permission;
    }
}
