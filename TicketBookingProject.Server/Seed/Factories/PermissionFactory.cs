using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public static class PermissionFactory
{
    private static Permission P(string resource, string action, string desc) => new()
    {
        Name = $"{resource}:{action}",
        Resource = resource,
        Action = action,
        Description = desc,
    };

    public static readonly List<Permission> AllPermissions =
    [
        // Event
        P(PermissionResource.Event,   PermissionAction.Create, "Tạo sự kiện mới"),
        P(PermissionResource.Event,   PermissionAction.Read,   "Xem sự kiện"),
        P(PermissionResource.Event,   PermissionAction.Update, "Cập nhật sự kiện"),
        P(PermissionResource.Event,   PermissionAction.Delete, "Xoá sự kiện"),
        P(PermissionResource.Event,   PermissionAction.Manage, "Quản lý toàn bộ sự kiện"),

        // Booking
        P(PermissionResource.Booking, PermissionAction.Create, "Tạo đặt vé"),
        P(PermissionResource.Booking, PermissionAction.Read,   "Xem đặt vé"),
        P(PermissionResource.Booking, PermissionAction.Update, "Cập nhật đặt vé"),
        P(PermissionResource.Booking, PermissionAction.Delete, "Huỷ đặt vé"),
        P(PermissionResource.Booking, PermissionAction.Manage, "Quản lý toàn bộ đặt vé"),

        // Ticket
        P(PermissionResource.Ticket,  PermissionAction.Read,   "Xem vé"),
        P(PermissionResource.Ticket,  PermissionAction.Update, "Cập nhật trạng thái vé"),
        P(PermissionResource.Ticket,  PermissionAction.Manage, "Quản lý toàn bộ vé"),

        // Venue
        P(PermissionResource.Venue,   PermissionAction.Create, "Tạo địa điểm"),
        P(PermissionResource.Venue,   PermissionAction.Read,   "Xem địa điểm"),
        P(PermissionResource.Venue,   PermissionAction.Update, "Cập nhật địa điểm"),
        P(PermissionResource.Venue,   PermissionAction.Delete, "Xoá địa điểm"),
        P(PermissionResource.Venue,   PermissionAction.Manage, "Quản lý toàn bộ địa điểm"),

        // User
        P(PermissionResource.User,    PermissionAction.Read,   "Xem người dùng"),
        P(PermissionResource.User,    PermissionAction.Update, "Cập nhật người dùng"),
        P(PermissionResource.User,    PermissionAction.Delete, "Xoá người dùng"),
        P(PermissionResource.User,    PermissionAction.Manage, "Quản lý toàn bộ người dùng"),

        // Payment
        P(PermissionResource.Payment, PermissionAction.Read,   "Xem thanh toán"),
        P(PermissionResource.Payment, PermissionAction.Manage, "Quản lý toàn bộ thanh toán"),

        // Refund
        P(PermissionResource.Refund,  PermissionAction.Read,   "Xem hoàn tiền"),
        P(PermissionResource.Refund,  PermissionAction.Update, "Xử lý hoàn tiền"),
        P(PermissionResource.Refund,  PermissionAction.Manage, "Quản lý toàn bộ hoàn tiền"),

        // Report
        P(PermissionResource.Report,  PermissionAction.Read,   "Xem báo cáo"),
        P(PermissionResource.Report,  PermissionAction.Manage, "Quản lý toàn bộ báo cáo"),
    ];

    private static Permission Get(string resource, string action)
        => AllPermissions.First(p => p.Resource == resource && p.Action == action);

    /// <summary>
    /// Permissions mặc định cho từng role, key = role.Name.
    /// </summary>
    public static readonly Dictionary<string, List<Permission>> RolePermissions = new()
    {
        ["admin"] =
        [
            // Manage tất cả
            ..AllPermissions.Where(p => p.Action == PermissionAction.Manage).ToList(),
            // Thêm các quyền CRUD cụ thể cho user management
            Get(PermissionResource.User,   PermissionAction.Read),
            Get(PermissionResource.User,   PermissionAction.Update),
            Get(PermissionResource.User,   PermissionAction.Delete),
            Get(PermissionResource.Report, PermissionAction.Read),
            Get(PermissionResource.Refund, PermissionAction.Update),
        ],

        ["organizer"] =
        [
            Get(PermissionResource.Event,   PermissionAction.Create),
            Get(PermissionResource.Event,   PermissionAction.Read),
            Get(PermissionResource.Event,   PermissionAction.Update),
            Get(PermissionResource.Event,   PermissionAction.Delete),
            Get(PermissionResource.Venue,   PermissionAction.Read),
            Get(PermissionResource.Booking, PermissionAction.Read),
            Get(PermissionResource.Ticket,  PermissionAction.Read),
            Get(PermissionResource.Payment, PermissionAction.Read),
            Get(PermissionResource.Report,  PermissionAction.Read),
        ],

        ["staff"] =
        [
            Get(PermissionResource.Event,   PermissionAction.Read),
            Get(PermissionResource.Ticket,  PermissionAction.Read),
            Get(PermissionResource.Ticket,  PermissionAction.Update),  // check-in
            Get(PermissionResource.Booking, PermissionAction.Read),
        ],

        ["customer"] =
        [
            Get(PermissionResource.Event,   PermissionAction.Read),
            Get(PermissionResource.Booking, PermissionAction.Create),
            Get(PermissionResource.Booking, PermissionAction.Read),
            Get(PermissionResource.Booking, PermissionAction.Delete),
            Get(PermissionResource.Ticket,  PermissionAction.Read),
            Get(PermissionResource.Payment, PermissionAction.Read),
            Get(PermissionResource.Refund,  PermissionAction.Read),
        ],
    };
}
