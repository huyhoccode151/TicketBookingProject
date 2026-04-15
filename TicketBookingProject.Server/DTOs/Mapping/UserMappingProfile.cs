using AutoMapper;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // ── RegisterRequest → User ────────────────────────────
        CreateMap<RegisterRequest, User>()
            .ForMember(d => d.Password, o => o.Ignore()); // hashed ở service
        // ── RegisterRequest → User ────────────────────────────
        CreateMap<RegisterUserRequest, User>()
            .ForMember(d => d.Password, o => o.Ignore()); // hashed ở service

        // ── User → RegisterResponse ───────────────────────────
        CreateMap<User, RegisterResponse>();

        // ── User → UserAuthDto ────────────────────────────────
        CreateMap<User, UserAuthDto>()
            .ForMember(d => d.Roles,
                o => o.MapFrom(s => s.Roles.Select(r => r.Name).ToList()))
            .ForMember(d => d.Permissions,
                o => o.MapFrom(s =>
                s.Roles.SelectMany(r => r.Permissions).Select(p => p.Name)
                    .Union(
                        s.UserPermissions.Where(up => up.Effect != -1).Select(up => up.Permission.Name)
                    )
                    .Except(
                        s.UserPermissions.Where(up => up.Effect == -1).Select(up => up.Permission.Name)
                    )
                    .Distinct()
                    .ToList()))
            .ForMember(d => d.Status,
                o => o.MapFrom(s => (byte)s.Status));

        // ── User → UserProfileResponse ────────────────────────
        CreateMap<User, UserProfileResponse>()
            .ForMember(d => d.Gender, o => o.MapFrom(s => (byte?)s.Gender))
            .ForMember(d => d.Status, o => o.MapFrom(s => (byte)s.Status))
            .ForMember(d => d.LoginType, o => o.MapFrom(s => (byte)s.LoginType))
            .ForMember(d => d.IsEmailVerified, o => o.MapFrom(s => s.EmailVerifiedAt != null));

        // ── User → UserListItemResponse ───────────────────────
        CreateMap<User, UserListItemResponse>()
            .ForCtorParam("Status", o => o.MapFrom(s => (byte)s.Status))
            .ForCtorParam("LoginType", o => o.MapFrom(s => (byte)s.LoginType))
            .ForCtorParam("IsEmailVerified", o => o.MapFrom(s => s.EmailVerifiedAt != null))
            .ForCtorParam("Roles", o => o.MapFrom(s => s.Roles.Select(r => r.Name).ToList()));
        // ── User → UserDetailResponse ─────────────────────────
        CreateMap<User, UserDetailResponse>()
            .ForMember(d => d.Gender, o => o.MapFrom(s => (byte?)s.Gender))
            .ForMember(d => d.Status, o => o.MapFrom(s => (byte)s.Status))
            .ForMember(d => d.LoginType, o => o.MapFrom(s => (byte)s.LoginType))
            .ForMember(d => d.IsEmailVerified, o => o.MapFrom(s => s.EmailVerifiedAt != null))
            .ForMember(d => d.Roles, o => o.MapFrom(s => s.Roles.Select(r => r.Name).ToList()))
            .ForMember(d => d.Permissions, o => o.MapFrom(s => 
                s.Roles.SelectMany(r => r.Permissions).Select(p => p.Name)
                    .Union(
                        s.UserPermissions.Where(up => up.Effect != -1).Select(up => up.Permission.Name)
                    )
                    .Except(
                        s.UserPermissions.Where(up => up.Effect == -1).Select(up => up.Permission.Name)
                    )
                    .Distinct()
                    .ToList()
            ));

        // -- CreateUserRequest → User (Admin) ───────────────────────────
        CreateMap<CreateUserRequest, User>()
            .ForMember(d => d.Status, o => o.MapFrom(_ => (byte)_.Status))
            .ForMember(d => d.Gender, o => o.MapFrom(_ => Gender.Unknown))
            .ForMember(d => d.LoginType, o => o.MapFrom(_ => LoginType.Email))
            .ForMember(d => d.Password, o => o.Ignore())
            .ForMember(d => d.Roles, o => o.Ignore());

        CreateMap<UpdateUserRequest, User>()
            .ForMember(opt => opt.Password, opt => opt.Ignore())
            .ForMember(d => d.Roles, o => o.Ignore())
            .ForAllMembers(opt =>
                opt.Condition((src, dest, srcMember) => srcMember != null)
            );

    }
}
