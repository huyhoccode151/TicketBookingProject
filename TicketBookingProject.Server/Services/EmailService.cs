using MailKit.Net.Smtp;
using MimeKit;
using QRCoder;

namespace TicketBookingProject.Server;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    public EmailService(IConfiguration config) { _config = config; }

    public async Task SendVerifyEmailNewUser(string toEmail, string customerName, string verifyUrl)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Hệ thống đặt vé", _config["EmailSettings:Email"]!));
        message.To.Add(new MailboxAddress(customerName, toEmail));
        message.Subject = $"Xác thực tài khoản #{customerName}";

        var builder = new BodyBuilder();

        builder.HtmlBody = $@"
            <div style='font-family: Arial; max-width: 600px; margin: auto;'>
                <h2 style='color: #007bff;'>Xác thực email</h2>
                <p>Cảm ơn bạn đã đăng ký! Vui lòng click vào nút bên dưới để xác thực:</p>
                <a href='{verifyUrl}' 
                   style='background:#007bff; color:white; padding:12px 24px; 
                          text-decoration:none; border-radius:4px; display:inline-block;'>
                    Xác thực tài khoản
                </a>
                <p style='color:#999; font-size:12px;'>Link hết hạn sau 24 giờ.</p>
            </div>";

        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_config["EmailSettings:Host"]!, int.Parse(_config["EmailSettings:Port"]!), MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_config["EmailSettings:Email"]!, _config["EmailSettings:Password"]!);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendTicketEmailAsync(string toEmail, string customerName, string bookingCode, List<TicketDetailResponse> tickets)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Hệ thống đặt vé", _config["EmailSettings:Email"]!));
        message.To.Add(new MailboxAddress(customerName, toEmail));
        message.Subject = $"Thanh toán thành công đơn hàng #{bookingCode}";

        var builder = new BodyBuilder();

        // Tạo HTML danh sách vé
        string ticketHtmlItems = "";
        QRCodeGenerator qrGenerator = new QRCodeGenerator();

        foreach (var ticket in tickets)
        {
            // Tạo QR Code cho từng vé
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(ticket.QrCode, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrImage = qrCode.GetGraphic(10);

            // Nhúng QR vào email
            var image = builder.LinkedResources.Add($"qr_{ticket.QrCode}.png", qrImage);
            image.ContentId = $"cid_{ticket.QrCode}";

            ticketHtmlItems += $@"
            <div style='border: 1px solid #eee; padding: 10px; margin-bottom: 10px; display: flex; align-items: center;'>
                <div style='flex: 1;'>
                    <p><b>Vé:</b> {ticket.TicketTypeName}</p>
                    <p><b>Ghế:</b> {ticket.SeatLabel ?? null}</p>
                    <p><b>Mã:</b> {ticket.QrCode}</p>
                </div>
                <div style='text-align: center;'>
                    <img src='cid:{image.ContentId}' width='120' />
                </div>
            </div>";
        }

        builder.HtmlBody = $@"
        <div style='font-family: Arial; max-width: 600px; margin: auto;'>
            <h2 style='color: #28a745;'>Cảm ơn bạn đã đặt vé!</h2>
            <p>Chào <b>{customerName}</b>, thanh toán cho đơn hàng <b>#{bookingCode}</b> đã thành công.</p>
            <hr/>
            <h4>Chi tiết vé của bạn:</h4>
            {ticketHtmlItems}
            <hr/>
            <p>Vui lòng xuất trình email này tại quầy để check-in.</p>
        </div>";

        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_config["EmailSettings:Host"]!, int.Parse(_config["EmailSettings:Port"]!), MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_config["EmailSettings:Email"]!, _config["EmailSettings:Password"]!);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
