using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketBookingProject.Server;
using TicketBookingProject.Server.Common.Extensions;
using TicketBookingProject.Server.Models;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        [HasPermission("permission:manage")]
        public async Task<IActionResult> GetListPermission([FromQuery] PermissionListRequest req)
        {
            var permissions = await _permissionService.GetListPermission(req);

            return Ok(ApiResponse<PagedResponse<PermissionResponseDto>>.Ok(permissions, "Load list permission successfully!!!"));
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [HasPermission("permission:create")]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionDto req)
        {
            var permission = await _permissionService.CreatePermission(req);

            return Ok(ApiResponse<PermissionResponseDto>.Ok(permission, "Create Permission successfully!!!"));
        }

        [HttpPost("toggle-role-permission")]
        [Authorize(Roles = "admin")]
        //[HasPermission("")]
        public async Task<IActionResult> ToggleRolePermission([FromBody] TogglePermissionDto req)
        {
            var permission = await _permissionService.ToggleRolePermission(req);

            return Ok(ApiResponse<bool>.Ok(permission, "Change status successfully!!!"));
        }

        [HttpGet("name")]
        [Authorize(Roles = "admin")]
        [HasPermission("permission:manage")]
        public async Task<IActionResult> GetPermissionName([FromQuery] string[]? Name = null)
        {
            var permission = await _permissionService.GetPermissionName(Name);

            return permission.ToActionResult();
        }
    }
}
