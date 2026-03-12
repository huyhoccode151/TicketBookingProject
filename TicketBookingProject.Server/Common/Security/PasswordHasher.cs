namespace TicketBookingProject.Server;

public static class PasswordHasher
{
    public static string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    public static bool Verify(string password, string passwordHashOrPasswordColumn)
        => BCrypt.Net.BCrypt.Verify(password, passwordHashOrPasswordColumn);
}
