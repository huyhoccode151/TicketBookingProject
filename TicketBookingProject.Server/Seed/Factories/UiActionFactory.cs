using Microsoft.VisualBasic.FileIO;

namespace TicketBookingProject.Server;

public static class UiActionFactory
{
    public static List<UiAction> SystemUiActions =>
    [
        // ── Events ────────────────────────────────────────
        new UiAction { ActionKey = "nav.events",               Label = "Events",              Icon = "event",                RoutePath = "/events",               PermissionRequired = "event:read",                ActionType = "nav",    DisplayOrder = 1,  IsActive = true, CreatedAt = DateTime.UtcNow },
        new UiAction { ActionKey = "btn.events.create",        Label = "Create Event",        Icon = "event_available",      RoutePath = "/events/create",        PermissionRequired = "event:create",              ActionType = "button", DisplayOrder = 1,  IsActive = true, CreatedAt = DateTime.UtcNow },
        new UiAction { ActionKey = "btn.events.update-status", Label = "Update Event Status", Icon = "published_with_changes",RoutePath = null,                   PermissionRequired = "event:update-status",       ActionType = "button", DisplayOrder = 2,  IsActive = true, CreatedAt = DateTime.UtcNow },
        new UiAction { ActionKey = "btn.events.confirm",       Label = "Confirm Event",       Icon = "check_circle",         RoutePath = null,                    PermissionRequired = "event:confirm",             ActionType = "button", DisplayOrder = 3,  IsActive = true, CreatedAt = DateTime.UtcNow },
        new UiAction { ActionKey = "btn.events.cancel",        Label = "Cancel Event",        Icon = "cancel",               RoutePath = null,                    PermissionRequired = "event:cancel",              ActionType = "button", DisplayOrder = 4,  IsActive = true, CreatedAt = DateTime.UtcNow },
        new UiAction { ActionKey = "nav.events.manage",        Label = "Manage Events",       Icon = "event_note",           RoutePath = "/events/manage",        PermissionRequired = "event:manage",              ActionType = "nav",    DisplayOrder = 2,  IsActive = true, CreatedAt = DateTime.UtcNow },
 
        // ── Bookings ──────────────────────────────────────
        new UiAction { ActionKey = "nav.bookings",             Label = "Bookings",            Icon = "book_online",          RoutePath = "/bookings",             PermissionRequired = "booking:read",              ActionType = "nav",    DisplayOrder = 3,  IsActive = true, CreatedAt = DateTime.UtcNow },
        new UiAction { ActionKey = "nav.bookings.my",          Label = "My Bookings",         Icon = "receipt_long",         RoutePath = "/bookings/my",          PermissionRequired = "booking:read-my-booking",   ActionType = "nav",    DisplayOrder = 4,  IsActive = true, CreatedAt = DateTime.UtcNow },
        new UiAction { ActionKey = "btn.bookings.create",      Label = "Create Booking",      Icon = "add_circle",           RoutePath = "/bookings/create",      PermissionRequired = "booking:create",            ActionType = "button", DisplayOrder = 1,  IsActive = true, CreatedAt = DateTime.UtcNow },
        new UiAction { ActionKey = "nav.bookings.manage",      Label = "Manage Bookings",     Icon = "bookmarks",            RoutePath = "/bookings/manage",      PermissionRequired = "booking:manage",            ActionType = "nav",    DisplayOrder = 5,  IsActive = true, CreatedAt = DateTime.UtcNow },
 
        // ── Tickets ───────────────────────────────────────
        new UiAction { ActionKey = "nav.tickets",              Label = "Tickets",             Icon = "confirmation_number",  RoutePath = "/tickets",              PermissionRequired = "ticket:read",               ActionType = "nav",    DisplayOrder = 6,  IsActive = true, CreatedAt = DateTime.UtcNow },
        new UiAction { ActionKey = "nav.tickets.manage",       Label = "Manage Tickets",      Icon = "local_activity",       RoutePath = "/tickets/manage",       PermissionRequired = "ticket:manage",             ActionType = "nav",    DisplayOrder = 7,  IsActive = true, CreatedAt = DateTime.UtcNow },
 
        // ── Venues ────────────────────────────────────────
        new UiAction { ActionKey = "nav.venues",               Label = "Venues",              Icon = "location_on",          RoutePath = "/venues",               PermissionRequired = "venue:read",                ActionType = "nav",    DisplayOrder = 8,  IsActive = true, CreatedAt = DateTime.UtcNow },
        new UiAction { ActionKey = "btn.venues.create",        Label = "Create Venue",        Icon = "add_location",         RoutePath = "/venues/create",        PermissionRequired = "venue:create",              ActionType = "button", DisplayOrder = 1,  IsActive = true, CreatedAt = DateTime.UtcNow },
        new UiAction { ActionKey = "nav.venues.manage",        Label = "Manage Venues",       Icon = "map",                  RoutePath = "/venues/manage",        PermissionRequired = "venue:manage",              ActionType = "nav",    DisplayOrder = 9,  IsActive = true, CreatedAt = DateTime.UtcNow },
 
        // ── Users ─────────────────────────────────────────
        new UiAction { ActionKey = "nav.users",                Label = "Users",               Icon = "group",                RoutePath = "/users",                PermissionRequired = "user:read",                 ActionType = "nav",    DisplayOrder = 10, IsActive = true, CreatedAt = DateTime.UtcNow },
        new UiAction { ActionKey = "btn.users.create",         Label = "Create User",         Icon = "person_add",           RoutePath = "/users/create",         PermissionRequired = "user:create",               ActionType = "button", DisplayOrder = 1,  IsActive = true, CreatedAt = DateTime.UtcNow },
        new UiAction { ActionKey = "nav.users.manage",         Label = "Manage Users",        Icon = "manage_accounts",      RoutePath = "/users/manage",         PermissionRequired = "user:manage",               ActionType = "nav",    DisplayOrder = 11, IsActive = true, CreatedAt = DateTime.UtcNow },
 
        // ── Roles ─────────────────────────────────────────
        new UiAction { ActionKey = "nav.roles",                Label = "Roles",               Icon = "admin_panel_settings", RoutePath = "/roles",                PermissionRequired = "role:read",                 ActionType = "nav",    DisplayOrder = 12, IsActive = true, CreatedAt = DateTime.UtcNow },
        new UiAction { ActionKey = "btn.roles.create",         Label = "Create Role",         Icon = "add_moderator",        RoutePath = "/roles/create",         PermissionRequired = "role:create",               ActionType = "button", DisplayOrder = 1,  IsActive = true, CreatedAt = DateTime.UtcNow },
        new UiAction { ActionKey = "nav.roles.manage",         Label = "Manage Roles",        Icon = "shield",               RoutePath = "/roles/manage",         PermissionRequired = "role:manage",               ActionType = "nav",    DisplayOrder = 13, IsActive = true, CreatedAt = DateTime.UtcNow },
 
        // ── Permissions ───────────────────────────────────
        new UiAction { ActionKey = "nav.permissions.manage",   Label = "Manage Permissions",  Icon = "lock",                 RoutePath = "/permissions/manage",   PermissionRequired = "permission:manage",         ActionType = "nav",    DisplayOrder = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
        new UiAction { ActionKey = "btn.permissions.create",   Label = "Create Permission",   Icon = "lock_open",            RoutePath = "/permissions/create",   PermissionRequired = "permission:create",         ActionType = "button", DisplayOrder = 1,  IsActive = true, CreatedAt = DateTime.UtcNow },
 
        // ── Payments ──────────────────────────────────────
        new UiAction { ActionKey = "nav.payments",             Label = "Payments",            Icon = "payments",             RoutePath = "/payments",             PermissionRequired = "payment:read",              ActionType = "nav",    DisplayOrder = 15, IsActive = true, CreatedAt = DateTime.UtcNow },
        new UiAction { ActionKey = "nav.payments.manage",      Label = "Manage Payments",     Icon = "point_of_sale",        RoutePath = "/payments/manage",      PermissionRequired = "payment:manage",            ActionType = "nav",    DisplayOrder = 16, IsActive = true, CreatedAt = DateTime.UtcNow },
 
        // ── Refunds ───────────────────────────────────────
        new UiAction { ActionKey = "nav.refunds",              Label = "Refunds",             Icon = "currency_exchange",    RoutePath = "/refunds",              PermissionRequired = "refund:read",               ActionType = "nav",    DisplayOrder = 17, IsActive = true, CreatedAt = DateTime.UtcNow },
        new UiAction { ActionKey = "btn.refunds.process",      Label = "Process Refund",      Icon = "replay",               RoutePath = null,                    PermissionRequired = "refund:update",             ActionType = "button", DisplayOrder = 1,  IsActive = true, CreatedAt = DateTime.UtcNow },
        new UiAction { ActionKey = "nav.refunds.manage",       Label = "Manage Refunds",      Icon = "price_check",          RoutePath = "/refunds/manage",       PermissionRequired = "refund:manage",             ActionType = "nav",    DisplayOrder = 18, IsActive = true, CreatedAt = DateTime.UtcNow },
 
        // ── Reports ───────────────────────────────────────
        new UiAction { ActionKey = "nav.reports",              Label = "Reports",             Icon = "bar_chart",            RoutePath = "/reports",              PermissionRequired = "report:read",               ActionType = "nav",    DisplayOrder = 19, IsActive = true, CreatedAt = DateTime.UtcNow },
        new UiAction { ActionKey = "nav.reports.manage",       Label = "Manage Reports",      Icon = "assessment",           RoutePath = "/reports/manage",       PermissionRequired = "report:manage",             ActionType = "nav",    DisplayOrder = 20, IsActive = true, CreatedAt = DateTime.UtcNow },
 
        // ── Audit Log ─────────────────────────────────────
        new UiAction { ActionKey = "nav.audit-log.manage",     Label = "Audit Logs",          Icon = "history",              RoutePath = "/audit-log",            PermissionRequired = "audit-log:manage",          ActionType = "nav",    DisplayOrder = 21, IsActive = true, CreatedAt = DateTime.UtcNow },
 
        // ── Event Log ─────────────────────────────────────
        new UiAction { ActionKey = "nav.event-log",            Label = "Event Statistics",    Icon = "insights",             RoutePath = "/event-log",            PermissionRequired = "event-log:view-event-view", ActionType = "nav",    DisplayOrder = 22, IsActive = true, CreatedAt = DateTime.UtcNow },
    ];
}
