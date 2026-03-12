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
            .ForMember(d => d.PermissionCount,
                o => o.MapFrom(s => s.Permissions.Count));

        // ── CreateRoleRequest → Role ──────────────────────────
        CreateMap<CreateRoleRequest, Role>()
            .ForMember(d => d.Permissions, o => o.Ignore());  // assigned separately
    }
}
