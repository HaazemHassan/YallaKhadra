namespace YallaKhadra.Core.Abstracts.ServicesContracts {
    public interface IEmailBodyBuilderService {
        string GenerateEmailBody(string templateName, Dictionary<string, string> templateModel);
    }
}
