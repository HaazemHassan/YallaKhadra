using YallaKhadra.Core.Abstracts.ServicesContracts;

namespace YallaKhadra.Infrastructure.BackgroundJobs.Jobs {
    public class SendEmailJob {
        private readonly IEmailService _emailService;

        public SendEmailJob(IEmailService emailService) {
            _emailService = emailService;
        }

        public async Task Execute(string email, string subject, string body) {
            await _emailService.SendEmailAsync(email, subject, body);
        }
    }
}
