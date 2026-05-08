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
    public string? Search { get; init; }
    public UserStatus? Status { get; init; }
    public string? Role { get; init; }
    public LoginType? LoginType { get; init; }
}

public record UserListItemResponse
    (
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
public class UserDetailResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public Gender? Gender { get; set; }
    public UserStatus Status { get; set; }
    public LoginType LoginType { get; set; }
    public bool? IsEmailVerified { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
    public List<string> Permissions { get; set; } = new List<string>();
    public DateTime CreatedAt { get; set; }
}
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
    [Required(ErrorMessage = "User Name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "User Name length must be at least 3 characters")]
    public string Username { get; init; } = default!;

    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Only Gmail address is allowed")]
    [StringLength(255, ErrorMessage = "Email is too long")]
    public string? Email { get; init; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password length must be at least 8 characters")]
    public string Password { get; init; } = default!;

    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("Password", ErrorMessage = "Pass word do not match")]
    public string ConfirmPassword { get; init; } = default!;

    [Required(ErrorMessage = "First Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "First Name is not valid")]
    public string Firstname { get; init; } = default!;

    [Required(ErrorMessage = "Last Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Last Name is not valid")]
    public string Lastname { get; init; } = default!;
    public Gender? Gender { get; init; }
    public UserStatus Status { get; init; }
    public LoginType LoginType { get; init; }
    public bool IsEmailVerified { get; init; } = false;

    [MinLength(1, ErrorMessage = "Select 1 role")]
    public List<string> Roles { get; init; } = [];
}

public record UpdateUserRequest
{
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Only Gmail address is allowed")]
    public string? Email { get; init; }

    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password length must be at least 8 characters")]
    public string? Password { get; init; } = default!;

    [Compare("Password", ErrorMessage = "Pass word do not match")]
    public string? ConfirmPassword { get; init; } = default!;

    [StringLength(100, MinimumLength = 1, ErrorMessage = "First Name is not valid")]
    public string? Firstname { get; init; } = default!;

    [StringLength(100, MinimumLength = 1, ErrorMessage = "Last Name is not valid")]
    public string? Lastname { get; init; } = default!;

    public Gender? Gender { get; init; }

    public UserStatus? Status { get; init; }

    public bool? IsEmailVerified { get; init; }

    public List<string>? Roles { get; init; }
}

public record AssignPermissionRequest
{
    [Required, MinLength(1)]
    public List<string> Permissions { get; init; } = [];
}

public class UserStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalUsersLastMonth { get; set; }

    public int NewUsersThisWeek { get; set; }
    public int NewUsersLastWeek { get; set; }
}

public record UpdateUserProfile
{
    [StringLength(100, MinimumLength = 1, ErrorMessage = "First Name is not valid")]
    public string? Firstname { get; init; } = default!;

    [StringLength(100, MinimumLength = 1, ErrorMessage = "Last Name is not valid")]
    public string? Lastname { get; init; } = default!;

    public Gender? Gender { get; init; }
}
