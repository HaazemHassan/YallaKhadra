using System.Text;
using YallaKhadra.Core.Abstracts.ServicesContracts;

namespace YallaKhadra.Services.Emails {
    public class EmailBodyBuilderService : IEmailBodyBuilderService {
        public string GenerateEmailBody(string templateName, Dictionary<string, string> templateModel) {
            if (string.IsNullOrWhiteSpace(templateName)) {
                throw new ArgumentException("Template name is required", nameof(templateName));
            }

            var templatePath = Path.Combine(AppContext.BaseDirectory, "Emails", "Templates", $"{templateName}.html");

            if (!File.Exists(templatePath)) {
                throw new FileNotFoundException($"Template not found: {templatePath}");
            }

            var body = File.ReadAllText(templatePath, Encoding.UTF8);

            if (templateModel == null || templateModel.Count == 0) {
                return body;
            }

            foreach (var item in templateModel) {
                var placeholder = $"{{{{{item.Key}}}}}";
                var value = item.Value ?? string.Empty;
                body = body.Replace(placeholder, value);
            }

            return body;
        }
    }
}
