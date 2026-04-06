namespace YallaKhadra.Core.Bases.Options {
    public class VerificationCodeOptions {
        public const string SectionName = "VerificationCodeOptions";

        public int EmailExpireInMinutes { get; set; } = 15;
        public int CodeLength { get; set; } = 6;
    }
}
