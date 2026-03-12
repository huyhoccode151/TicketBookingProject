namespace TicketBookingProject.Server;

public enum BookingStatus : byte
{
    Pending = 0,
    Confirmed = 1,
    Cancelled = 2,
    Expired = 3,
    Refunded = 4,
}

public enum SeatHoldStatus : byte
{
    Active = 0,
    Released = 1,
    Converted = 2,   // → booking confirmed
    Expired = 3,
}

public enum TicketStatus : byte
{
    Valid = 0,
    Used = 1,
    Cancelled = 2,
    Expired = 3,
}
