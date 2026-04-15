using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface IPermissionRepository : IBaseRepository<Permission>
{
    Task<List<Permission>> GetPermissionsByName(List<string> ListNamePermissions);
    Task<Permission?> GetPermissionByName(string name);
}
