using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TicketBookingProject.Server;

public record RegisterRequest
{
    [Required(ErrorMessage = "User Name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "User Name length must be at least 3 characters")]
    public string Username { get; init; } = default!;

    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Only Gmail address is allowed")]
    [StringLength(255, ErrorMessage = "Email is too long")]
    public string Email { get; init; } = default!;

    [Required(ErrorMessage = "First Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "First Name is not valid")]
    public string Firstname { get; init; } = default!;

    [Required(ErrorMessage = "Last Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Last Name is not valid")]
    public string Lastname { get; init; } = default!;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password length must be at least 8 characters")]
    public string Password { get; init; } = default!;

    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("Password", ErrorMessage = "Pass word do not match")]
    public string ConfirmPassword { get; init; } = default!;
}

public record RegisterResponse(
    int Id,
    string Username,
    string Email,
    string Firstname,
    string Lastname);

// ─────────────────────────────────────────────
// LOGIN
// ─────────────────────────────────────────────

public record LoginRequest
{
    [Required(ErrorMessage = "User name is required!!! Please enter username or email.")]
    public string UsernameOrEmail { get; init; } = default!;

    [Required(ErrorMessage = "Password is required!!! Please enter password.")]
    public string Password { get; init; } = default!;

    public string? DeviceInfo { get; init; }
}

public class GoogleLoginRequest
{
    public string IdToken { get; set; } = default!;
}

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt,
    UserAuthDto User);

// ─────────────────────────────────────────────
// REFRESH TOKEN
// ─────────────────────────────────────────────

public record RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; init; } = default!;
}

public record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt);

// ─────────────────────────────────────────────
// LOGOUT
// ─────────────────────────────────────────────

public record LogoutRequest
{
    [Required]
    public string RefreshToken { get; init; } = default!;
}

// ─────────────────────────────────────────────
// CHANGE PASSWORD
// ─────────────────────────────────────────────

public record ChangePasswordRequest
{
    [Required(ErrorMessage = "Current password is required")]
    public string CurrentPassword { get; init; } = default!;

    [Required(ErrorMessage = "New password is required"), StringLength(100, MinimumLength = 8, ErrorMessage = "New password length must be at least 8 characters")]
    public string NewPassword { get; init; } = default!;

    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("NewPassword", ErrorMessage = "Password do not match")]
    public string ConfirmPassword { get; init; } = default!;
}

// ─────────────────────────────────────────────
// FORGOT / RESET PASSWORD
// ─────────────────────────────────────────────

public record ForgotPasswordRequest
{
    [Required, EmailAddress]
    public string Email { get; init; } = default!;
}

public record ResetPasswordRequest
{
    [Required]
    public string Token { get; init; } = default!;

    [Required, EmailAddress]
    public string Email { get; init; } = default!;

    [Required, StringLength(100, MinimumLength = 8)]
    public string NewPassword { get; init; } = default!;

    [Required, Compare(nameof(NewPassword))]
    public string ConfirmPassword { get; init; } = default!;
}

// ─────────────────────────────────────────────
// VERIFY EMAIL
// ─────────────────────────────────────────────

public record VerifyEmailRequest
{
    [Required]
    public string Token { get; init; } = default!;

    [Required, EmailAddress]
    public string Email { get; init; } = default!;
}

// ─────────────────────────────────────────────
// Embedded user info in auth responses
// ─────────────────────────────────────────────

public record UserAuthDto(
    int Id,
    string Username,
    string Email,
    string Firstname,
    string Lastname,
    UserStatus Status,
    List<string> Roles,
    List<string> Permissions);

public class TokenResponse 
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}

public class ForceChangePasswordRequest
{
    [Required, StringLength(100, MinimumLength = 8)]
    public string NewPassword { get; init; } = default!;
}

public record AuditLogRequest : PagedRequest
{
    public string? Search { get; init; }
}

public class AuditUserDto
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
}

public class AuditLogDto
{
    public long Id { get; set; }
    public string Action { get; set; } = null!;
    public string EntityType { get; set; } = null!;
    public long? EntityId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public AuditUserDto? User { get; set; }
}