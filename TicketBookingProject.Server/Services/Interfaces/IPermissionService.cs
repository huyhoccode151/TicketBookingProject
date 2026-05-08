namespace TicketBookingProject.Server;

public interface IPermissionService
{
    Task<PagedResponse<PermissionResponseDto>> GetListPermission(PermissionListRequest req);
    Task<PermissionResponseDto> CreatePermission(CreatePermissionDto req);
    Task<bool> ToggleRolePermission(TogglePermissionDto req);
    Task<Result<List<string>>> GetPermissionName(string[] name);
}
