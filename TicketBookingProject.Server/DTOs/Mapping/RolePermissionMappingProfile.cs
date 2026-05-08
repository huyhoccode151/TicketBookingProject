using AutoMapper;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class RolePermissionMappingProfile : Profile
{
    public RolePermissionMappingProfile()
    {
        // ── Permission → PermissionResponse ──────────────────
        CreateMap<Permission, PermissionResponse>();

        // ── Role → RoleResponse ───────────────────────────────
        CreateMap<Role, RoleResponse>()
            .ForMember(d => d.Permissions,
                o => o.MapFrom(s => s.Permissions));

        // ── Role → RoleListResponse ───────────────────────────
        CreateMap<Role, RoleListResponse>()
            .ForCtorParam("PermissionCount",
                o => o.MapFrom(s => s.Permissions.Count));

        // ── CreateRoleRequest → Role ──────────────────────────
        CreateMap<CreateRoleRequest, Role>()
            .ForMember(d => d.Permissions, o => o.Ignore());  // assigned separately

        CreateMap<Permission, PermissionResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Action, opt => opt.MapFrom(src => src.Action))
            .ForMember(dest => dest.Resource, opt => opt.MapFrom(src => src.Resource))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.RoleStates, opt => opt.MapFrom(src =>
                src.Roles.ToDictionary(r => r.Id, r => true)
            ));
    }
}
