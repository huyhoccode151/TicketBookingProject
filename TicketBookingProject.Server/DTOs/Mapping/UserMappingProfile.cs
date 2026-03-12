using AutoMapper;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // ── RegisterRequest → User ────────────────────────────
        CreateMap<RegisterRequest, User>()
            .ForMember(d => d.Status, o => o.MapFrom(_ => UserStatus.Active))
            .ForMember(d => d.LoginType, o => o.MapFrom(_ => LoginType.Email))
            .ForMember(d => d.Password, o => o.Ignore())   // hashed ở service
            .ForMember(d => d.Gender, o => o.MapFrom(s => (Gender?)s.Gender));

        // ── User → RegisterResponse ───────────────────────────
        CreateMap<User, RegisterResponse>();

        // ── User → UserAuthDto ────────────────────────────────
        CreateMap<User, UserAuthDto>()
            .ForMember(d => d.Roles,
                o => o.MapFrom(s => s.Roles.Select(r => r.Name).ToList()))
            .ForMember(d => d.Permissions,
                o => o.MapFrom(s => s.Roles
                    .SelectMany(r => r.Permissions)
                    .Select(p => p.Name)
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
            .ForMember(d => d.Status, o => o.MapFrom(s => (byte)s.Status))
            .ForMember(d => d.LoginType, o => o.MapFrom(s => (byte)s.LoginType))
            .ForMember(d => d.IsEmailVerified, o => o.MapFrom(s => s.EmailVerifiedAt != null))
            .ForMember(d => d.Roles, o => o.MapFrom(s => s.Roles.Select(r => r.Name).ToList()));

        // ── User → UserDetailResponse ─────────────────────────
        CreateMap<User, UserDetailResponse>()
            .ForMember(d => d.Gender, o => o.MapFrom(s => (byte?)s.Gender))
            .ForMember(d => d.Status, o => o.MapFrom(s => (byte)s.Status))
            .ForMember(d => d.LoginType, o => o.MapFrom(s => (byte)s.LoginType))
            .ForMember(d => d.IsEmailVerified, o => o.MapFrom(s => s.EmailVerifiedAt != null))
            .ForMember(d => d.Roles, o => o.MapFrom(s => s.Roles.Select(r => r.Name).ToList()))
            .ForMember(d => d.Permissions, o => o.MapFrom(s => s.Roles
                .SelectMany(r => r.Permissions)
                .Select(p => p.Name)
                .Distinct()
                .ToList()));
        // -- CreateUserRequest → User (Admin) ───────────────────────────
        CreateMap<CreateUserRequest, User>()
            .ForMember(d => d.Status, o => o.MapFrom(_ => UserStatus.Active))
            .ForMember(d => d.Gender, o => o.MapFrom(_ => Gender.Unknown))
            .ForMember(d => d.LoginType, o => o.MapFrom(_ => LoginType.Email))
            .ForMember(d => d.Password, o => o.Ignore());
    }
}
