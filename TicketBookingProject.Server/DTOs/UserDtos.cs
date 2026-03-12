using System.ComponentModel.DataAnnotations;
using TicketBookingProject.Server;

namespace TicketBookingProject.Server;

// ─────────────────────────────────────────────
// USER REGISTER
// ─────────────────────────────────────────────
public record RegisterUserRequest
{
    [Required, StringLength(100, MinimumLength = 3)]
    public string Username { get; init; } = default!;

    [EmailAddress]
    public string? Email { get; init; }

    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; init; } = default!;

    [StringLength(100)]
    public string Firstname { get; init; } = default!;

    [StringLength(100)]
    public string Lastname { get; init; } = default!;

    [Range(0, 3)]
    public byte? Gender { get; init; }

    [Range(0, 2)]
    public byte Status { get; init; } = 1;
}

// ─────────────────────────────────────────────
// USER PROFILE
// ─────────────────────────────────────────────
public record UserProfileResponse(
    int Id,
    string Username,
    string? Email,
    string Firstname,
    string Lastname,
    byte? Gender,
    byte Status,
    byte LoginType,
    bool IsEmailVerified,
    DateTime CreatedAt);

// ─────────────────────────────────────────────
// USER UPDATE PROFILE
// ─────────────────────────────────────────────

public record UpdateProfileRequest
{
    [StringLength(100, MinimumLength = 1)]
    public string? Firstname { get; init; }

    [StringLength(100, MinimumLength = 1)]
    public string? Lastname { get; init; }

    [Range(0, 3)]
    public byte? Gender { get; init; }
}

// ─────────────────────────────────────────────
// ADMIN — LIST USERS
// ─────────────────────────────────────────────

public record UserListRequest : PagedRequest
{
    public string? Search { get; init; }   // username hoặc email
    public byte? Status { get; init; }
    public string? Role { get; init; }
    public byte? LoginType { get; init; }
}

public record UserListItemResponse(
    int Id,
    string Username,
    string? Email,
    string Firstname,
    string Lastname,
    UserStatus Status,
    LoginType LoginType,
    bool IsEmailVerified,
    List<string> Roles,
    DateTime? CreatedAt);

// ─────────────────────────────────────────────
// ADMIN — USER DETAIL
// ─────────────────────────────────────────────

public record UserDetailResponse(
    int Id,
    string Username,
    string? Email,
    string Firstname,
    string Lastname,
    Gender? Gender,
    UserStatus Status,
    LoginType LoginType,
    bool? IsEmailVerified,
    List<string> Roles,
    List<string> Permissions,
    DateTime? CreatedAt,
    DateTime? UpdatedAt);

// ─────────────────────────────────────────────
// ADMIN — UPDATE USER STATUS
// ─────────────────────────────────────────────

public record UpdateUserStatusRequest
{
    [Required, Range(0, 2)]
    public byte Status { get; init; }

    public string? Reason { get; init; }
}

// ─────────────────────────────────────────────
// ADMIN — ASSIGN ROLES
// ─────────────────────────────────────────────

public record AssignRolesRequest
{
    [Required, MinLength(1)]
    public List<string> Roles { get; init; } = [];
}

// ─────────────────────────────────────────────
// ADMIN — ASSIGN USERS
// ─────────────────────────────────────────────
public record CreateUserRequest
{
    [Required, StringLength(100, MinimumLength = 3)]
    public string Username { get; init; } = default!;

    [EmailAddress]
    public string? Email { get; init; }

    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; init; } = default!;

    [StringLength(100)]
    public string Firstname { get; init; } = default!;

    [StringLength(100)]
    public string Lastname { get; init; } = default!;

    [Range(0, 3)]
    public byte? Gender { get; init; }

    [Range(0, 2)]
    public byte Status { get; init; } = 1;

    [Range(0, 3)]
    public byte LoginType { get; init; } = 0;

    public bool IsEmailVerified { get; init; } = false;

    [MinLength(1)]
    public List<string> Roles { get; init; } = [];
}

public record UpdateUserRequest
{
    [EmailAddress]
    public string? Email { get; init; }

    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; init; } = default!;

    [StringLength(100)]
    public string Firstname { get; init; } = default!;

    [StringLength(100)]
    public string Lastname { get; init; } = default!;

    [Range(0, 3)]
    public byte? Gender { get; init; }

    [Range(0, 2)]
    public byte Status { get; init; } = 1;

    public bool IsEmailVerified { get; init; } = false;

    [MinLength(1)]
    public List<string> Roles { get; init; } = [];
}