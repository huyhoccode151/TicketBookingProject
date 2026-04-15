using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketBookingProject.Server;

namespace MyApp.Namespace
{
    [Route("api/auth/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest user, CancellationToken ct)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            return Ok(await _authService.Login(user, ip, ct));
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

            var permissions = User.FindAll("permission")
                .Select(p => p.Value)
                .ToList();

            return Ok(new
            {
                userId,
                username,
                permissions
            });
        }
    }
}
