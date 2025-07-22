using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MyBackend.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
    }

    public class EmailSender : IEmailSender
    {
        private readonly string _smtpHost = "smtp.gmail.com";      // Gmail SMTP
        private readonly int _smtpPort = 587;                       // TLS port
        private readonly string _smtpUser = "your-email@gmail.com"; // Replace with actual
        private readonly string _smtpPass = "your-app-password";    // Replace with actual

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            using var client = new SmtpClient(_smtpHost)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true,
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}
