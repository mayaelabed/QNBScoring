using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using QNBScoring.Core.Configurations;
using QNBScoring.Core.Interfaces;
using System.Threading.Tasks;

namespace QNBScoring.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("QNB Scoring", _emailSettings.User));
            message.To.Add(new MailboxAddress(to, to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, _emailSettings.EnableSsl);
            await client.AuthenticateAsync(_emailSettings.User, _emailSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
