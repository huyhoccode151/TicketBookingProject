using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface IPermissionRepository : IBaseRepository<Permission>
{
    Task<List<Permission>> GetPermissionsByName(List<string> ListNamePermissions);
    Task<Permission?> GetPermissionByName(string name);
    Task<(List<Permission>, int TotalCount)> GetListPermission(PermissionListRequest req);
    Task<Permission> CreatePermission(CreatePermissionDto req);
    Task<bool> TogglePermissionDto(TogglePermissionDto req);
    Task<IQueryable<Permission>> GetPermissionName(string[] name);
}
