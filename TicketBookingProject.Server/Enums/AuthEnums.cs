namespace TicketBookingProject.Server;

public enum RefreshTokenStatus : byte
{
    Active = 0,
    Revoked = 1,
    Expired = 2,
}

public static  class PermissionAction
{
    public const string Create = "create";
    public const string Read = "read";
    public const string Update = "update";
    public const string Delete = "delete";
    public const string Manage = "manage";   // full access (supersedes all above)
}

public static class PermissionResource
{
    public const string Event = "event";
    public const string Booking = "booking";
    public const string Ticket = "ticket";
    public const string Venue = "venue";
    public const string User = "user";
    public const string Payment = "payment";
    public const string Refund = "refund";
    public const string Report = "report";
}