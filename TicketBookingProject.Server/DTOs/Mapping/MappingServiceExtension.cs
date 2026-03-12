namespace TicketBookingProject.Server;

public static class MappingServiceExtension
{
    public static IServiceCollection AddMappings(this IServiceCollection services)
    {
        services.AddAutoMapper(
            typeof(UserMappingProfile),
            typeof(RolePermissionMappingProfile),
            typeof(VenueMappingProfile),
            typeof(EventMappingProfile),
            typeof(BookingMappingProfile),
            typeof(TicketMappingProfile),
            typeof(PaymentMappingProfile),
            typeof(RefundMappingProfile)
        );

        return services;
    }
}
