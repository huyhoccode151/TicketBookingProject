namespace TicketBookingProject.Server;

public interface IEmailService
{
    Task SendVerifyEmailNewUser(string toEmail, string customerName, string verifyUrl);
    Task SendTicketEmailAsync(string toEmail, string customerName, string bookingCode, List<TicketDetailResponse> tickets);
}
