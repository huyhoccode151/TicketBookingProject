using Microsoft.EntityFrameworkCore;
using TicketBookingProject.Server.Models;

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
        var permissions = await _db.Users.Where(u => u.Id == userId)
            .SelectMany(u => u.Roles)
            .SelectMany(r => r.Permissions)
            .Select(p => p.Name)
            .ToListAsync(ct);
        return permissions;
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> GetByUserNameAsync(string username, CancellationToken ct = default)
    => await _db.Users.Where(u => u.Username == username).FirstOrDefaultAsync(ct);

    public string? GetJwtConfig(string key) => _cfg.GetSection("Jwt")[key];
    public async Task<User?> UpdateAsync(User user, CancellationToken ct = default)
    {
        var existing = await _db.Users.FindAsync(new object[] { user.Id }, ct);
        if (existing == null)
            return null;

        _db.Entry(existing).CurrentValues.SetValues(user);

        //_db.Users.Update(user);
        await _db.SaveChangesAsync(ct);
        return existing;
    }
    public async Task<User> CreateAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        return user;
    }
    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await this.GetAllAsync();
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

}
