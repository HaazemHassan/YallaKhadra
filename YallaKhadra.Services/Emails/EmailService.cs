using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Options;

namespace YallaKhadra.Services.Emails {
    public class EmailService : IEmailService {
        private readonly MailOptions _mailSettings;

        public EmailService(IOptions<MailOptions> mailSettings) {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage) {
            var message = new MimeMessage {
                Sender = MailboxAddress.Parse(_mailSettings.Mail),
                Subject = subject,
            };

            message.To.Add(MailboxAddress.Parse(email));
            message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));

            var builder = new BodyBuilder {
                HtmlBody = htmlMessage
            };

            message.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}
