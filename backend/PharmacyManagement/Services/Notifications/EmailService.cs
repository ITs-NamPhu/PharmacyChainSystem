using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace PharmacyManagement.Services.Notifications
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml)
        {
            // Đọc cấu hình từ appsettings
            string host = _config["EmailSettings:Host"]!;
            int port = int.Parse(_config["EmailSettings:Port"]!);
            string fromEmail = _config["EmailSettings:Email"]!;
            string password = _config["EmailSettings:AppPassword"]!;
            string displayName = _config["EmailSettings:DisplayName"]!;

            // Sử dụng (SMTP Client) để gửi thông báo thông qua Gmail
            using var smtpClient = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true // Giao thức bảo mật bắt buộc của Gmail
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, displayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}