namespace YallaKhadra.Core.Bases.Options {
    public class MailOptions {
        public const string SectionName = "MailOptions";

        public string DisplayName { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
    }
}
