using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketBookingProject.Server;
using TicketBookingProject.Server.Common.Extensions;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        [HasPermission("role:manage")]
        public async Task<IActionResult> GetRoles([FromQuery] ListRoleRequest request)
        {
            var roles = await _roleService.GetAllRoles(request);

            return roles.ToActionResult();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [HasPermission("role:create")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            var result = await _roleService.CreateRole(request);

            return result.ToActionResult();
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "admin")]
        [HasPermission("role:update")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleRequest request)
        {
            var result = await _roleService.UpdateRole(id, request);

            return result.ToActionResult();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        [HasPermission("role:read")]
        public async Task<IActionResult> GetRole(int id)
        {
            var result = await _roleService.GetRoleById(id);

            return result.ToActionResult();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        [HasPermission("role:delete")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var result = await _roleService.DeleteRole(id);

            return result.ToActionResult();
        }

    }
}
