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

    public async Task<(IQueryable<Role>, int TotalCount)> GetListRole(ListRoleRequest req)
    {
        var roles = _dbset.AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Keyword)) roles = roles.Where(r => r.Name == req.Keyword);

        if (req.PermissionNames != null && req.PermissionNames.Any(p => !string.IsNullOrWhiteSpace(p)))
        {
            roles = roles.Where(r => r.Permissions.Any(p => req.PermissionNames.Contains(p.Name)));
        }

        var totalCount = await roles.CountAsync();

        roles = roles.OrderByDescending(r => r.Id).Skip((req.Page - 1) * req.PageSize).Take(req.PageSize);

        return (roles, totalCount);
    }

    public async Task<Role?> CreateRole(CreateRoleRequest req)
    {
        var exists = await _dbset.AnyAsync(r => r.Name == req.Name);
        if (exists) return null;

        var role = new Role
        {
            Name = req.Name,
            Description = req.Description,
        };

        var toAdd = await _db.Permissions
            .Where(p => req.PermissionNames.Contains(p.Name))
            .ToListAsync();

        foreach (var p in toAdd)
        {
            role.Permissions.Add(p);
        }

        _dbset.Add(role);
        await _db.SaveChangesAsync();
        return role;
    }

    public async Task<Role?> UpdateRole(int id, UpdateRoleRequest req)
    {
        var exists = await _dbset.Include(r => r.Permissions).Where(r => r.Id == id).FirstOrDefaultAsync();
        if (exists == null) return null;

        exists.Description = req.Description ?? exists.Description;
        
        var current = exists.Permissions.Select(p => p.Name).ToHashSet();
        var incoming = req.PermissionNames != null ? req.PermissionNames.ToHashSet() : new HashSet<string>();

        var toRemove = exists.Permissions
            .Where(p => !incoming.Contains(p.Name))
            .ToList();

        foreach(var p in toRemove)
        {
            exists.Permissions.Remove(p);
        }

        var toAddNames = incoming.Except(current);

        var toAdd = await _db.Permissions
            .Where(p => toAddNames.Contains(p.Name))
            .ToListAsync();

        foreach (var p in toAdd)
        {
            exists.Permissions.Add(p);
        }

        await _db.SaveChangesAsync();
        return exists;
    }

    public async Task<RoleResponse?> GetRoleById(int id)
    {
        var role = await _dbset.Where(r => r.Id == id)
            .Select(r => new RoleResponse(
                r.Id,
                r.Name,
                r.Description,
                r.Permissions.Select(p => new PermissionResponse(
                    p.Id,
                    p.Name,
                    p.Action,
                    p.Resource,
                    p.Description
                )).ToList()
            )).FirstOrDefaultAsync();
        if (role == null) return null;

        return role;
    }

    public async Task<bool> DeleteRole(int id)
    {
        var role = await _dbset.FindAsync(id);
        if (role == null) return false;

        var hasUsers = await _db.Users.AnyAsync(ur => ur.Roles.Any(r => r.Id == id));
        var hasPermissions = await _db.Permissions.AnyAsync(rp => rp.Roles.Any(r => r.Id == id));

        if (hasUsers || hasPermissions) return false;

        _dbset.Remove(role);
        await _db.SaveChangesAsync();
        return true;
    }
}
