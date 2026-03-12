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
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; init; } = default!;

    [StringLength(255)]
    public string? Description { get; init; }

    public List<int> PermissionIds { get; init; } = [];
}

public record UpdateRoleRequest
{
    [StringLength(255)]
    public string? Description { get; init; }

    public List<int>? PermissionIds { get; init; }
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

public record PermissionListRequest
{
    public string? Resource { get; init; }
    public string? Action { get; init; }
}

public record AssignPermissionsToRoleRequest
{
    [Required, MinLength(1)]
    public List<int> PermissionIds { get; init; } = [];
}
