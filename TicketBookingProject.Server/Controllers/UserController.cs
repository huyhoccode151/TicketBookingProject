using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketBookingProject.Server;
using TicketBookingProject.Server.Common.Extensions;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _user;
        public UserController(IUserService user)
        {
            _user = user;
        }

        
        [HttpGet]
        [Authorize(Roles = "admin")]
        [HasPermission("user:manage")]
        public async Task<IActionResult> ListUser([FromQuery] UserListRequest req)
        {
            var pagedUsers = await _user.GetListUserAsync(req);
            
            return pagedUsers.ToActionResult();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ShowUserDetail(int id)
        {
            var user = await _user.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(ApiResponse<UserDetailResponse>.Ok(user, "Show user success!!!"));
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "admin")]
        [HasPermission("user:update")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest req)
        {
            var userUpdated = await _user.UpdateUserAsync(id, req);
            
            return userUpdated.ToActionResult();
        }

        [HttpPatch("{id}/password")]
        public async Task<IActionResult> ForceChangePassword(int id, [FromBody] ForceChangePasswordRequest req)
        {
            var user = await _user.ForceChangePassword(id, req);
            return Ok(ApiResponse.Ok("Password changed"));
        }

        [HttpPatch("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
        {
            var user = await _user.ChangePassword(req);

            return user.ToActionResult();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [HasPermission("user:create")]
        public async Task<IActionResult> AddUser([FromBody] CreateUserRequest req)
        {
            var user = await _user.StoreUserAsync(req);

            return user.ToActionResult();
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest req)
        {
            var user = await _user.RegisterUserAsync(req);

            return user.ToActionResult();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        [HasPermission("user:delete")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _user.DeleteUserAsync(id);

            return user.ToActionResult();
        }

        [HttpPost("{id}/permissions")]
        [Authorize(Roles = "admin")]
        [HasPermission("permission:manage")]
        public async Task<IActionResult> AssignPermissionToUser(int id, [FromBody] AssignPermissionRequest req)
        {
            var user = await _user.AssignPermissionToUserAsync(id, req);
            if (user == null) return NotFound();
            return Ok(ApiResponse<UserDetailResponse>.Ok(user, "Assign permission to user successfully!!!"));
        }

        [HttpGet("{id}/permissions")]
        public async Task<IActionResult> GetPermissionsByUserId(int id)
        {
            var permissions = await _user.GetPermissionsByUserIdAsync(id);
            return Ok(ApiResponse<List<string>>.Ok(permissions, "Get permissions by user id successfully!!!"));
        }

        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetRolePermissionByUserId(int id)
        {
            var roles = await _user.GetRolePermissionByUserIdAsync(id);
            return Ok(ApiResponse<List<string>>.Ok(roles, "Get roles by user id successfully!!!"));
        }

        [HttpGet("stats")]
        [Authorize(Roles = "admin")]
        [HasPermission("user:manage")]
        public async Task<IActionResult> GetStatUsers()
        {
            var userStats = await _user.GetUserStats();
            return Ok(ApiResponse< UserStatsDto >.Ok(userStats, "Get user stats successfully!!!"));
        }

        [HttpGet("name")]
        public async Task<IActionResult> GetUserName([FromQuery] string? req)
        {
            var userNames = await _user.GetUserName(req);

            return Ok(ApiResponse<List<string>>.Ok(userNames, "Load User name search successfully!!!"));
        }

        [HttpPatch("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfile req)
        {
            var user = await _user.UpdateUserProfileAsync(req);
            return user.ToActionResult();
        }
    }
}
