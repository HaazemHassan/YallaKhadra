namespace YallaKhadra.Core.Abstracts.ServicesContracts {
    public interface IEmailService {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
