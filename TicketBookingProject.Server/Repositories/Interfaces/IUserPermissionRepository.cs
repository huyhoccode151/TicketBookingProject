using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface IUserPermissionRepository : IBaseRepository<UserPermission>
{
    Task AddUserPermissionAsync(UserPermission userPermission, CancellationToken ct = default);

    Task<List<Permission>> GetPermissionsByUserIdAsync(int userId, CancellationToken ct = default);
}
