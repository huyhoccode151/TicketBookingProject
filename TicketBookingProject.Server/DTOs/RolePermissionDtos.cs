using System.ComponentModel.DataAnnotations;

namespace TicketBookingProject.Server;

public record RoleResponse(
    int Id,
    string Name,
    string? Description,
    List<PermissionResponse> Permissions);

public record RoleListResponse(
    int Id,
    string Name,
    string? Description,
    int PermissionCount);

public record CreateRoleRequest
{
    [Required(ErrorMessage = "Role name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 100 characters")]
    public string Name { get; init; } = default!;

    [StringLength(255, ErrorMessage = "Description can not exceed 255 characters")]
    public string? Description { get; init; }

    [Required(ErrorMessage = "Permission is required")]
    public List<string> PermissionNames { get; init; } = [];
}

public record UpdateRoleRequest
{
    [StringLength(255, ErrorMessage = "Description can not exceed 255 characters")]
    public string? Description { get; init; }

    public List<string>? PermissionNames { get; init; }
}

// ─────────────────────────────────────────────
// PERMISSION
// ─────────────────────────────────────────────

public record PermissionResponse(
    int Id,
    string Name,
    string Action,
    string Resource,
    string? Description);

public record PermissionListRequest : PagedRequest
{
    public string? Resource { get; init; }
    public string? Action { get; init; }
}

public record AssignPermissionsToRoleRequest
{
    [Required(ErrorMessage = "Permission IDs are required"), MinLength(1, ErrorMessage = "At least one permission ID must be provided")]
    public List<int> PermissionIds { get; init; } = [];
}

public class PermissionResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    // roleId -> bool, tổng quát cho bao nhiêu role cũng được
    public Dictionary<int, bool> RoleStates { get; set; } = new();
}
public class CreatePermissionDto
{
    [Required(ErrorMessage = "Permission action is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Permission action must be between 2 and 100 characters")]
    public string Action {  set; get; } = string.Empty;
    [Required(ErrorMessage = "Permission resource is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Permission resource must be between 2 and 100 characters")]
    public string Resource {  set; get; } = string.Empty;
    [StringLength(255, ErrorMessage = "Description can not exceed 255 characters")]
    public string? Description { get; set; }
}

public class TogglePermissionDto
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
    public bool IsSelected { get; set; }
}

public record ListRoleRequest : PagedRequest
{
    public string? Keyword { get; init; }

    public List<string>? PermissionNames { get; init; }
}