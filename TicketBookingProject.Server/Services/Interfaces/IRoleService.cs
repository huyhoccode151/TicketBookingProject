namespace TicketBookingProject.Server;

public interface IRoleService
{
    Task<Result<PagedResponse<RoleListResponse>>> GetAllRoles(ListRoleRequest req);
    Task<Result<RoleResponse>> CreateRole(CreateRoleRequest req);
    Task<Result<RoleResponse>> UpdateRole(int id, UpdateRoleRequest req);
    Task<Result<RoleResponse>> GetRoleById(int id);
    Task<Result<bool>> DeleteRole(int id);
}
