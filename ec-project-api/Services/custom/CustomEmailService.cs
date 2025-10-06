using System.Net;
using System.Net.Mail;

namespace ec_project_api.Services
{
    public class CustomEmailService
    {
        private readonly IConfiguration _config;

        public CustomEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            var host = _config["Email:Host"];
            var port = int.Parse(_config["Email:Port"]!);
            var username = _config["Email:Username"];
            var password = _config["Email:Password"];
            var fromName = _config["Email:FromName"];

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = bool.Parse(_config["Email:EnableSSL"] ?? "true")
            };

            using var message = new MailMessage
            {
                From = new MailAddress(username!, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            message.To.Add(to);

            await client.SendMailAsync(message);
        }
    }
}
