using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name);
    Task<List<Role>> GetListRoleByListString(List<string> names);
    Task<List<string>> GetPermissionsByRole(List<int> roles);
    Task<List<Permission>> GetPermissionsByUserIdAsync(int userId, CancellationToken ct = default);
    Task<(IQueryable<Role>, int TotalCount)> GetListRole(ListRoleRequest req);
    Task<Role?> CreateRole(CreateRoleRequest req);
    Task<Role?> UpdateRole(int id, UpdateRoleRequest req);
    Task<RoleResponse?> GetRoleById(int id);
    Task<bool> DeleteRole(int id);
}
