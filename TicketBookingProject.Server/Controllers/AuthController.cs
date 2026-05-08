using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketBookingProject.Server;
using TicketBookingProject.Server.Common.Extensions;

namespace MyApp.Namespace
{
    [Route("api/auth/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        public AuthController(IAuthService authService, IUserService userService) {
            _authService = authService;
            _userService = userService;
        } 

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest user, CancellationToken ct)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            var result = await _authService.Login(user, ip, ct);
            return result.ToActionResult();
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var result = await _authService.GoogleLogin(request);

            return result.ToActionResult();
        }

        [HttpGet("verify")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string userName)
        {
            var user = await _userService.VerifyEmail(userName);

            return user.ToActionResult();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody]RefreshTokenRequest rfToken, CancellationToken ct)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            return Ok(await _authService.RefreshToken(rfToken.RefreshToken, ip, ct));
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userId = User.FindFirst("userId")?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            var roles = User.FindAll(ClaimTypes.Role)
                .Select(p => p.Value)
                .ToList();

            var permissions = User.FindAll("permission")
                .Select(p => p.Value)
                .ToList();

            return Ok(new
            {
                userId,
                username,
                roles,
                permissions
            });
        }
    }
}
