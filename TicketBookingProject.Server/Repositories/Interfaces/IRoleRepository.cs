using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name);
    Task<List<Role>> GetListRoleByListString(List<string> names);
    Task<List<string>> GetPermissionsByRole(List<int> roles);
    Task<List<Permission>> GetPermissionsByUserIdAsync(int userId, CancellationToken ct = default);
}
