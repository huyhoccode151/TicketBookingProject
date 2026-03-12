using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public static class RoleFactory
{
    public static readonly List<Role> SystemRoles =
    [
        new() { Name = "admin",     Description = "Quản trị viên hệ thống"  },
        new() { Name = "organizer", Description = "Nhà tổ chức sự kiện"     },
        new() { Name = "staff",     Description = "Nhân viên soát vé"        },
        new() { Name = "customer",  Description = "Khách hàng"               },
    ];
}
