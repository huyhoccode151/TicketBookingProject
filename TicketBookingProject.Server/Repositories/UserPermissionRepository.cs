using Microsoft.EntityFrameworkCore;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class UserPermissionRepository : BaseRepository<UserPermission>, IUserPermissionRepository
{
    public UserPermissionRepository(TicketBookingProjectContext db) : base(db)
    {
    }
     public async Task<List<Permission>> GetPermissionsByUserIdAsync(int userId, CancellationToken ct = default)
     {
         var permissions = await _dbset.Where(up => up.UserId == userId)
             .Select(up => up.Permission)
             .ToListAsync(ct);
         return permissions;
     }
    public async Task AddUserPermissionAsync(UserPermission userPermission, CancellationToken ct = default)
    {
        await _dbset.AddAsync(userPermission, ct);
        await _db.SaveChangesAsync(ct);
    }
}
