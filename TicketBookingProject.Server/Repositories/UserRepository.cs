using Microsoft.EntityFrameworkCore;
using TicketBookingProject.Server.Models;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;

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
    //=> await _db.Users.Where(u => u.Username == username).FirstOrDefaultAsync(ct);
    {
        return await _dbset.Where(u => u.Username == username).Select(u => new User
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            Firstname = u.Firstname,
            Lastname = u.Lastname,
            Password = u.Password,
            AccessFailedCount = u.AccessFailedCount,
            Gender = u.Gender,
            Status = u.Status,
            LoginType = u.LoginType,
            EmailVerifiedAt = u.EmailVerifiedAt,
            LockoutEnd = u.LockoutEnd,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt,
            DeletedAt = u.DeletedAt,
        }).FirstOrDefaultAsync(ct);
    }

    public async Task<List<string>> GetUserRolesAsync(int userId, CancellationToken ct = default)
    {
        var roleName = await _dbset.Where(u => u.Id == userId).SelectMany(u => u.Roles.Select(r => r.Name)).ToListAsync(ct);

        return roleName;
    }

    public string? GetJwtConfig(string key) => _cfg.GetSection("Jwt")[key];
    public async Task<User?> UpdateAsync(User user, CancellationToken ct = default)
    {
        await _db.SaveChangesAsync(ct);
        return user;
    }
    public async Task<User> CreateAsync(User user, CancellationToken ct = default)
    {
        user.CreatedAt = DateTime.UtcNow;
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        return user;
    }
    public async Task<(IQueryable<User>, int TotalCount)> GetAllUsers(UserListRequest req, bool IsSuperAdmin = false)
    {
        var users = _db.Users.Include(u => u.Roles).ThenInclude(r => r.Permissions).AsQueryable();

        if (!IsSuperAdmin) users = users.Where(u => !u.Roles.Any(r => r.Name == "admin"));

        users = users.Where(u => u.Status != UserStatus.SuperAdmin);

        if (!string.IsNullOrWhiteSpace(req.Search))
        {
            users = users.Where(x =>
            x.Username.Contains(req.Search) ||
            x.Email!.Contains(req.Search));
        }

        if (req.Status != null)
        {
            users = users.Where(x => x.Status == req.Status);
        }

        if (req.Role != null)
        {
            users = users.Where(r => r.Roles.Any(n => n.Name == req.Role));
        }

        if (req.LoginType != null)
        {
            users = users.Where(l => l.LoginType == req.LoginType);
        }    

        //if (!string.IsNullOrWhiteSpace(req.SortBy))
        //{
        //    if (req.SortDesc) users = users.OrderBy($"{req.SortBy} descending");
        //    else users = users.OrderBy(req.SortBy);
        //}
        //else
        //{
        //    if (req.SortDesc) users = users.OrderByDescending(x => x.CreatedAt);
        //    else users = users.OrderBy(x => x.CreatedAt);
        //}

        var total = await users.CountAsync();

        users = users.OrderByDescending(x => x.CreatedAt)
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize);

        return (users, total);
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

    public async Task<List<string>> GetUserName(string? req)
    {
        if (!string.IsNullOrWhiteSpace(req)) return await _dbset.Where(u => EF.Functions.Like(u.Username ?? "", $"%{req}%")).Take(10).Select(u => u.Username).ToListAsync();
        
        return await _dbset.Select(u => u.Username).ToListAsync();
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _dbset.Where(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task<UserAuthDto> VerifyEmail(User user)
    {
        user.EmailVerifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return new UserAuthDto
        (
            user.Id,
            user.Username,
            user.Email!,
            user.Firstname,
            user.Lastname,
            user.Status,
            user.Roles.Select(r => r.Name).ToList(),
            user.UserPermissions.Where(u => u.UserId == user.Id).Select(up => up.Permission.Name).ToList()
        );
    }

    public async Task UserEvent(int id)
    {
        var user = _dbset.Include(u => u.Events).Where(u => u.Id == id)
            .Select(u => new
            {
                Id = u.Id,
                FirstName = u.Firstname,
                LastName = u.Lastname,
                Events = u.Events.OrderByDescending(e => e.CreatedAt).Select(e => e.Name).Take(3).ToList()
            })
            .FirstOrDefault();
    }
}
