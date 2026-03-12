namespace TicketBookingProject.Server;

public enum EventStatus : byte
{
    Draft = 0,
    Published = 1,
    Ongoing = 2,
    Completed = 3,
    Cancelled = 4,
}

public enum ImageType : byte
{
    Poster = 0,
    Banner = 1,
    Thumbnail = 2,
}

public enum TicketTypeStatus : byte
{
    Hidden = 0,
    OnSale = 1,
    SoldOut = 2,
    Paused = 3,
}

public enum EventSeatStatus : byte
{
    Available = 0,
    Reserved = 1,   // held, awaiting payment
    Sold = 2,
    Locked = 3,   // admin locked
}
