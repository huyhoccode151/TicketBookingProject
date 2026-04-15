using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net.WebSockets;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    public RoleRepository(TicketBookingProjectContext db) : base(db)
    {
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        var role = _db.Roles.FirstOrDefault(x => x.Name == name);
        return role;
    }

    public async Task<List<Role>> GetListRoleByListString(List<string> names)
    {
        var roles = await _db.Roles.Where(r => names.Contains(r.Name)).ToListAsync();
        return roles;
    }

    public async Task<List<string>> GetPermissionsByRole(List<int> roles)
    {
        var permissions = await _db.Roles.Where(r => roles.Contains(r.Id)).SelectMany(p => p.Permissions).Distinct().Select(o => o.Name).ToListAsync();
        return permissions;
    }

    public async Task<List<Permission>> GetPermissionsByUserIdAsync(int userId, CancellationToken ct = default)
    {
        var permissions = await _db.Users.Where(u => u.Id == userId)
            .SelectMany(u => u.Roles)
            .SelectMany(r => r.Permissions)
            .ToListAsync(ct);
        return permissions;
    }
}
