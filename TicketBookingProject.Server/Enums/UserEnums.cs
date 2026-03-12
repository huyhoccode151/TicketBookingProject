namespace TicketBookingProject.Server;

public enum Gender : byte
{
    Unknown = 0,
    Male = 1,
    Female = 2,
    Other = 3,
}

public enum UserStatus : byte
{
    Inactive = 0,
    Active = 1,
    Banned = 2,
}

public enum LoginType : byte
{
    Email = 1,
    Google = 2,
    Facebook = 3,
}
