using Microsoft.EntityFrameworkCore;
using System.Security;
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

    public async Task<(List<Permission>, int TotalCount)> GetListPermission(PermissionListRequest req)
    {
        var permissions = _dbset
                    .Include(p => p.Roles)
                    .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Action)) permissions = permissions.Where(p => p.Action == req.Action);

        if (!string.IsNullOrWhiteSpace(req.Resource)) permissions = permissions.Where(p => p.Resource == req.Resource);

        var total = await permissions.CountAsync();

        if (req.SortDesc) permissions = permissions.OrderByDescending(b => b.Name);

        var items = await permissions.Skip((req.Page - 1) * req.PageSize).Take(req.PageSize).ToListAsync();

        return (items, total);
    }

    public async Task<Permission> CreatePermission(CreatePermissionDto req)
    {
        var permissionName = $"{req.Resource.ToLower()}:{req.Action.ToLower()}";

        var exists = await _dbset.AnyAsync(p => p.Name == permissionName);
        if (exists) return new Permission();

        var permission = new Permission
        {
            Name = permissionName,
            Action = req.Action,
            Resource = req.Resource,
            Description = req.Description,
        };

        _dbset.Add(permission);
        _db.SaveChanges();

        return permission;
    }

    public async Task<bool> TogglePermissionDto(TogglePermissionDto req)
    {
        var role = await _db.Roles
        .Include(r => r.Permissions)
        .FirstOrDefaultAsync(r => r.Id == req.RoleId);

        if (role == null) return false;

        // 2. Tìm permission
        var permission = await _db.Permissions
            .FirstOrDefaultAsync(p => p.Id == req.PermissionId);

        if (permission == null) return false;

        // 3. Kiểm tra đã có chưa
        var hasPermission = role.Permissions.Any(p => p.Id == req.PermissionId);

        if (req.IsSelected)
        {
            if (!hasPermission)
            {
                role.Permissions.Add(permission);
            }
        }
        else
        {
            if (hasPermission)
            {
                role.Permissions.Remove(permission);
            }
        }

        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<IQueryable<Permission>> GetPermissionName(string[] name)
    {
        var permission = _dbset.AsQueryable();

        if (name != null) permission = permission.Where(p => name.Contains(p.Name));

        return permission;
    }
}
