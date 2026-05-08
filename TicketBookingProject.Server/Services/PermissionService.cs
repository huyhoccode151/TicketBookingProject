using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepo;
    private readonly IMapper _mapper;
    public PermissionService(IPermissionRepository permissionRepo, IMapper mapper) {  
        _permissionRepo = permissionRepo;
        _mapper = mapper;
    }
    public async Task<PagedResponse<PermissionResponseDto>> GetListPermission(PermissionListRequest req)
    {
        var (permissions, total) = await _permissionRepo.GetListPermission(req);

        var mapped = _mapper.Map<List<PermissionResponseDto>>(permissions);

        return new PagedResponse<PermissionResponseDto>(
                    mapped,
                    req.Page,
                    req.PageSize,
                    total
                );
    }

    public async Task<PermissionResponseDto> CreatePermission(CreatePermissionDto req)
    {
        var permission = await _permissionRepo.CreatePermission(req);

        return _mapper.Map<PermissionResponseDto>(permission);
    }

    public async Task<bool> ToggleRolePermission(TogglePermissionDto req)
    {
        var permission = await _permissionRepo.TogglePermissionDto(req);

        return permission;
    }

    public async Task<Result<List<string>>> GetPermissionName(string[] name)
    {
        var permission = await _permissionRepo.GetPermissionName(name);

        if (permission == null) return Result<List<string>>.Failure("Permission not found");

        var result = permission.Select(p => p.Name).ToList();

        return Result<List<string>>.Success(result, "Permissions Name restrived successfully");
    }
}
