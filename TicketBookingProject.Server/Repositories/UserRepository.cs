using Microsoft.EntityFrameworkCore;
using TicketBookingProject.Server.Models;
using System.Linq.Dynamic.Core;

namespace TicketBookingProject.Server;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    private IConfiguration _cfg;
    public UserRepository(IConfiguration cfg, TicketBookingProjectContext db) : base(db)
    {
        _cfg = cfg;
    }

    public async Task<List<string>> GetPermissionsByUserIdAsync(int userId, CancellationToken ct = default)
    {
        var rolePermissions = await _db.Users.Where(u => u.Id == userId)
            .SelectMany(u => u.Roles)
            .SelectMany(r => r.Permissions)
            .Select(p => p.Name)
            .ToListAsync(ct);

        var userPermissions = await _db.UserPermissions.Where(up => up.UserId == userId)
            .Include(up => up.Permission)
            .ToListAsync(ct);

        var finalPermissions = rolePermissions
            .Union(userPermissions.Where(up => up.Effect != -1).Select(up => up.Permission.Name))
            .Except(userPermissions.Where(up => up.Effect == -1).Select(up => up.Permission.Name))
            .Distinct()
            .ToList();
        return finalPermissions;
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Users.Include(u => u.Roles).ThenInclude(r => r.Permissions).FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> GetByUserNameAsync(string username, CancellationToken ct = default)
    => await _db.Users.Where(u => u.Username == username).FirstOrDefaultAsync(ct);

    public string? GetJwtConfig(string key) => _cfg.GetSection("Jwt")[key];
    public async Task<User?> UpdateAsync(User user, CancellationToken ct = default)
    {
        await _db.SaveChangesAsync(ct);
        return user;
    }
    public async Task<User> CreateAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        return user;
    }
    public async Task<PagedResponse<User>> GetAllUsers(UserListRequest req)
    {
        var query = _db.Users.Include(u => u.Roles).AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Search))
        {
            query = query.Where(x =>
            x.Username.Contains(req.Search) ||
            x.Email!.Contains(req.Search));
        }

        if (req.Status != null)
        {
            query = query.Where(x => x.Status == req.Status);
        }

        if (req.Role != null)
        {
            query = query.Where(r => r.Roles.Any(n => n.Name == req.Role));
        }

        if (req.LoginType != null)
        {
            query = query.Where(l => l.LoginType == req.LoginType);
        }    

        if (!string.IsNullOrWhiteSpace(req.SortBy))
        {
            if (req.SortDesc) query = query.OrderBy($"{req.SortBy} descending");
            else query = query.OrderBy(req.SortBy);
        }
        else
        {
            if (req.SortDesc) query = query.OrderByDescending(x => x.CreatedAt);
            else query = query.OrderBy(x => x.CreatedAt);
        }

        var total = await query.CountAsync();

        var users = await query.OrderByDescending(x => x.CreatedAt) //note: hien dang trung orderby, sau sua lai
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .ToListAsync();

        return new PagedResponse<User>(users, req.Page, req.PageSize, total);
    }
    public async Task<bool> SoftDeleteUser(User user, CancellationToken ct = default)
    {
        user.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }
    public async Task<bool> ForceDeleteUser(User user, CancellationToken ct = default)
    {
        await this.ForceDeleteAsync(user);
        await _db.SaveChangesAsync(ct);
        return true;
    }
    public virtual async Task<int> MultiSoftDelete(List<int> ids)
    {
        return await _dbset.Where(e => ids.Contains(e.Id)).ExecuteUpdateAsync(e => e.SetProperty(p => p.DeletedAt, DateTime.UtcNow));
    }
    public virtual async Task<int> MultiRestoreUser(List<int> ids)
    {
        return await _dbset.Where(e => ids.Contains(e.Id)).ExecuteUpdateAsync(e => e.SetProperty(p => p.DeletedAt, (DateTime?)null));
    }

    public async Task<int> MultiForceDeleteUser(List<int> ids, CancellationToken ct = default)
    {
        var count = await this.MultiForceDelete(ids);

        return count;
    }

    public Task<List<string>> GetUserRolesAsync(int userId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>> GetRolePermissions(int userId, CancellationToken ct = default)
    {
        var rolePermissions = _db.Users.Where(u => u.Id == userId)
            .SelectMany(u => u.Roles)
            .SelectMany(r => r.Permissions)
            .Select(p => p.Name).ToList();

        return Task.FromResult(rolePermissions);
    }

    public async Task<UserStatsDto> GetUserStats()
    {
        var now = DateTime.UtcNow;
        var thisMonday = now.AddDays(-(int)now.DayOfWeek + 1).Date;
        var lastMonday = thisMonday.AddDays(-7);

        return new UserStatsDto
        {
            TotalUsers = await _dbset.CountAsync(),
            TotalUsersLastMonth = await _dbset.CountAsync(u => u.CreatedAt <= now.AddDays(-30)),

            NewUsersThisWeek = await _dbset.CountAsync(u => u.CreatedAt >= thisMonday),
            NewUsersLastWeek = await _dbset.CountAsync(u => u.CreatedAt < thisMonday && u.CreatedAt >= lastMonday),
        };
    }
}
