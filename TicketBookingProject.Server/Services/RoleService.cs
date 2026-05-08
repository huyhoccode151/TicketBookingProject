using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace TicketBookingProject.Server;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    public RoleService(IRoleRepository roleRepository, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<Result<PagedResponse<RoleListResponse>>> GetAllRoles( ListRoleRequest req)
    {
        var (roles, total) = await _roleRepository.GetListRole(req);

        if (roles == null || !roles.Any())
        {
            return Result<PagedResponse<RoleListResponse>>.Success( new PagedResponse<RoleListResponse>(new List<RoleListResponse>(), req.Page, req.PageSize, 0), "No roles found.");
        }

        var pagedResponses = await roles.ProjectTo<RoleListResponse>(_mapper.ConfigurationProvider).ToListAsync();

        var result = new PagedResponse<RoleListResponse>
        (
            pagedResponses,
            req.Page,
            req.PageSize,
            total
        );

        return Result<PagedResponse<RoleListResponse>>.Success(result, "Get List Role Successfully"); ;
    }

    public async Task<Result<RoleResponse>> CreateRole(CreateRoleRequest req)
    {
        var createdRole = await _roleRepository.CreateRole(req);
        if (createdRole == null)
        {
            return Result<RoleResponse>.Failure("Failed to create role // Role is already existed");
        }
        var roleResponse = _mapper.Map<RoleResponse>(createdRole);
        return Result<RoleResponse>.Success(roleResponse, "Role created successfully.");
    }

    public async Task<Result<RoleResponse>> UpdateRole(int id, UpdateRoleRequest req)
    {
        var updatedRole = await _roleRepository.UpdateRole(id, req);
        if (updatedRole == null)
        {
            return Result<RoleResponse>.Failure("Failed to update role // Role is not existed");
        }
        var roleResponse = _mapper.Map<RoleResponse>(updatedRole);
        return Result<RoleResponse>.Success(roleResponse, "Role updated successfully.");
    }

    public async Task<Result<RoleResponse>> GetRoleById(int id) {
        var role = await _roleRepository.GetRoleById(id);

        if (role == null) return Result<RoleResponse>.Failure("Failed to load role");

        return Result<RoleResponse>.Success(role, "Role Loaded Success.");
    }

    public async Task<Result<bool>> DeleteRole(int id)
    {
        var deleted = await _roleRepository.DeleteRole(id);
        if (!deleted)
        {
            return Result<bool>.Failure("Failed to delete role // Role is not existed // Role has associated users or permissions", StatusCodes.Status409Conflict);
        }
        return Result<bool>.Success(deleted, "Role deleted successfully.");
    }
}
