using AutoMapper;
using Azure.Core;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using TicketBookingProject.Server.Enums;
using TicketBookingProject.Server.Models;
using static QRCoder.PayloadGenerator;

namespace TicketBookingProject.Server;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IRefreshTokenRepository _rfToken;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _cfg;
    private readonly IAuditLogService _auditLog;
    private readonly IMapper _mapper;
    public AuthService(IUserRepository users, IRefreshTokenRepository rfToken, ITokenService tokenService, IConfiguration cfg, IAuditLogService auditLog, IRoleRepository roles, IMapper mapper)
    {
        _users = users;
        _rfToken = rfToken;
        _tokenService = tokenService;
        _cfg = cfg;
        _auditLog = auditLog;
        _roles = roles;
        _mapper = mapper;
    }

    public async Task<Result<LoginResponse>> Login(LoginRequest user, string? ip = null, CancellationToken ct = default)
    {
        var foundUser = await _users.GetByUserNameAsync(user.UsernameOrEmail); //tam thoi tim theo username
        
        if (foundUser == null) return Result<LoginResponse>.Failure("Account is not exist", StatusCodes.Status401Unauthorized);
        if (foundUser.EmailVerifiedAt == null) return Result<LoginResponse>.Failure("Account even not verified yet!!!", StatusCodes.Status401Unauthorized);

        if (foundUser.LockoutEnd > DateTime.UtcNow) 
            return Result<LoginResponse>.Failure("Account is locked. " +
                $"Please try again after {Math.Ceiling((foundUser.LockoutEnd!.Value - DateTime.UtcNow).TotalMinutes)} minutes.", StatusCodes.Status401Unauthorized);

        if (!PasswordHasher.Verify(user.Password, foundUser.Password!))
        {
            foundUser.AccessFailedCount++;
            if (foundUser.AccessFailedCount > 5)
            {
                foundUser.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                foundUser.AccessFailedCount = 0;
            }
            await _users.UpdateAsync(foundUser);
            //throw new Exception("Password is incorrect");
            return Result<LoginResponse>.Failure("Password is incorrect", StatusCodes.Status401Unauthorized);
        }

        foundUser.AccessFailedCount = 0;
        foundUser.LockoutEnd = null;

        var permissions = await _users.GetPermissionsByUserIdAsync(foundUser.Id, ct);
        var roles = await _users.GetUserRolesAsync(foundUser.Id, ct);

        var accessToken = _tokenService.CreateAccessToken(foundUser, permissions, roles);
        var refreshToken = GenerateRefreshToken();
        string hashedRefreshToken = HashSHA256(refreshToken);

        await _rfToken.CreateAsync(new RefreshToken
        {
            UserId = foundUser.Id,
            Token = hashedRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            Status = RefreshTokenStatus.Active,
            IpAddress = ip,
        },ct);

        _auditLog.AddLog(
            AuditAction.Login,
            "Login",
            foundUser.Id,
            $"Account {foundUser.Username} login successfully!!!",
            new
            {
                foundUser.Username,
                foundUser.Email,
            },
            foundUser.Id
            );
        await _users.SaveChanges();

        var response =  new LoginResponse(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(15),
            DateTime.UtcNow.AddDays(7),
            new UserAuthDto(
                    foundUser.Id,
                    foundUser.Username,
                    foundUser.Email ?? "",
                    foundUser.Firstname,
                    foundUser.Lastname,
                    foundUser.Status,
                    foundUser.Roles.Select(r => r.Name).Take(3).ToList(),
                    foundUser.UserPermissions.Select(p => p.Permission.Name).Take(10).ToList()
                ));

        return Result<LoginResponse>.Success( response );
    }

    public async Task<Result<LoginResponse>> GoogleLogin(GoogleLoginRequest request, string? ip = null, CancellationToken ct = default)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _cfg["OAuthClientId:GoogleClientId"]! }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

            var email = payload.Email;
            var name = payload.Name;

            var user = await _users.GetUserByEmail(email);
            var userCreated = new User();
            if (user == null)
            {
                var newUser = new User
                {
                    Username = name,
                    Firstname = name,
                    Lastname = name,
                    Email = email,
                    Gender = Gender.Unknown,
                    Status = UserStatus.Active,
                    LoginType = LoginType.Google,
                    EmailVerifiedAt = DateTime.UtcNow,
                    Roles = await _roles.GetListRoleByListString(["customer"]),
                };

                userCreated = await _users.CreateAsync(newUser);
                if (userCreated == null) return Result<LoginResponse>.Failure("Login User By Google Failed!!!", StatusCodes.Status403Forbidden);
            }

            var userLogin = user ?? userCreated;

            var permissions = await _users.GetPermissionsByUserIdAsync(userLogin.Id, ct);
            var roles = await _users.GetUserRolesAsync(userLogin.Id, ct);

            var accessToken = _tokenService.CreateAccessToken(userLogin, permissions, roles);
            var refreshToken = GenerateRefreshToken();
            string hashedRefreshToken = HashSHA256(refreshToken);

            await _rfToken.CreateAsync(new RefreshToken
            {
                UserId = userLogin.Id,
                Token = hashedRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                Status = RefreshTokenStatus.Active,
                IpAddress = ip,
            }, ct);

            var response = new LoginResponse(
                accessToken,
                refreshToken,
                DateTime.UtcNow.AddMinutes(15),
                DateTime.UtcNow.AddDays(7),
                new UserAuthDto(
                        userLogin.Id,
                        userLogin.Username,
                        userLogin.Email ?? "",
                        userLogin.Firstname,
                        userLogin.Lastname,
                        userLogin.Status,
                        userLogin.Roles.Select(r => r.Name).Take(3).ToList(),
                        userLogin.UserPermissions.Select(p => p.Permission.Name).Take(10).ToList()
                    ));

            return Result<LoginResponse>.Success(response, StatusCodes.Status201Created, "Account registered!!!");

        }
        catch (Exception ex) {
            return Result<LoginResponse>.Failure("Login By Google Failed!!!", StatusCodes.Status204NoContent);
        }
    }

    private string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private string HashSHA256(string input)
    {
        using var sha256 = SHA256.Create();
        return Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(input)));
    }

    public async Task<TokenResponse> RefreshToken(string tokenRes, string? ip = null, CancellationToken ct = default)
    {
        var hashedRfToken = HashSHA256(tokenRes);

        // 2. Kiểm tra Refresh Token trong Database
        var storedToken = await _rfToken.CheckToken(hashedRfToken);
        if (storedToken == null) throw new Exception("Refresh Token expired or not exist!!!");

        // 3. Vô hiệu hóa Token cũ (Security Best Practice: Rotation)
        storedToken.Status = RefreshTokenStatus.Revoked; // Đánh dấu là đã sử dụng/revoked
        storedToken.RevokedAt = DateTime.UtcNow;

        // 4. Lấy lại danh sách Permission của User để tạo Access Token mới
        var userId = storedToken.UserId;
        var user = await _users.GetByIdAsync(userId);
        if (user == null) throw new Exception("User does not exist");

        var permissions = await _users.GetPermissionsByUserIdAsync(userId);
        var roles = await _users.GetUserRolesAsync(userId, ct);

        // 5. Tạo cặp Token mới
        var newAccessToken = _tokenService.CreateAccessToken(user, permissions, roles);
        var newRefreshToken = GenerateRefreshToken();

        // 6. Lưu Refresh Token mới vào DB
        int rfDays = _cfg.GetValue<int>("Jwt:RefreshTokenDays", 7);
        await _rfToken.CreateAsync(new RefreshToken
        {
            UserId = userId,
            Token = HashSHA256(newRefreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(rfDays),
            CreatedAt = DateTime.UtcNow,
            Status = RefreshTokenStatus.Active,
            IpAddress = ip,
        }, ct);

        return new TokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
}
