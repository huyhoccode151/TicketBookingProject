using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketBookingProject.Server;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _user;
        public UserController(IUserService user) => _user = user;

        [HttpGet]
        public async Task<IActionResult> ListUser([FromQuery] UserListRequest req)
        {
            var pagedUsers = await _user.GetListUserAsync(req);
            if (pagedUsers == null) return NotFound(ApiResponse<PagedResponse<UserListItemResponse>>.Fail("Cant found any user"));
            return Ok(ApiResponse<PagedResponse<UserListItemResponse>>.Ok(pagedUsers));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ShowUserDetail(int id)
        {
            var user = await _user.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(ApiResponse<UserDetailResponse>.Ok(user, "Show user success!!!"));
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest req)
        {
            var userUpdated = await _user.UpdateUserAsync(id, req);
            if (userUpdated == null) return NotFound();
            return Ok(ApiResponse<UserDetailResponse>.Ok(userUpdated, "Update successed!!!"));
        }

        [HttpPatch("{id}/password")]
        public async Task<IActionResult> ForceChangePassword(int id, [FromBody] ForceChangePasswordRequest req)
        {
            var user = await _user.ForceChangePassword(id, req);
            return Ok(ApiResponse.Ok("Password changed"));
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] CreateUserRequest req)
        {
            var user = await _user.StoreUserAsync(req);
            if (user == null) return Ok();
            return Ok(ApiResponse<UserDetailResponse>.Ok(user, "User added successfully!!!"));
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest req)
        {
            var user = await _user.RegisterUserAsync(req);
            if (user == null) return Ok();
            return Ok(ApiResponse<UserDetailResponse>.Ok(user, "User added successfully!!!"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _user.DeleteUserAsync(id);
            return Ok(ApiResponse.Ok("Success deleted!!!"));
        }

        [HttpPost("{id}/permissions")]
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
        public async Task<IActionResult> GetStatUsers()
        {
            var userStats = await _user.GetUserStats();
            return Ok(ApiResponse< UserStatsDto >.Ok(userStats, "Get user stats successfully!!!"));
        }
    }
}
