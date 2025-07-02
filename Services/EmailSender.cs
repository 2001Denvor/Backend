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
        private readonly string _smtpHost = "smtp.gmail.com"; // ✅ Correct host for Gmail
        private readonly int _smtpPort = 587;                 // ✅ Correct port (587 for TLS/STARTTLS)
        private readonly string _smtpUser = "your-email@gmail.com"; // ⚠️ Must replace with actual Gmail
        private readonly string _smtpPass = "your-app-password";    // ⚠️ Must replace with actual app password

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var client = new SmtpClient(_smtpHost)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true,      // ✅ Enable SSL/TLS
            };

            var mailMessage = new MailMessage
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
