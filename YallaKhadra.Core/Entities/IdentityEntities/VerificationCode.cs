using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Entities.IdentityEntities {
    public class VerificationCode {
        public const int MaxAttempts = 3;

        private VerificationCode() {
        }

        public VerificationCode(
            int applicationUserId,
            string code,
            VerificationCodeType type,
            DateTime expiresAt) {
            ApplicationUserId = applicationUserId;
            Code = code;
            Type = type;
            ExpiresAt = expiresAt;
            Attempts = 0;
            Status = VerificationCodeStatus.Active;
            CreatedAt = DateTime.UtcNow;
        }

        public int Id { get; private set; }
        public int ApplicationUserId { get; private set; }
        public string Code { get; private set; } = string.Empty;
        public VerificationCodeType Type { get; private set; }
        public VerificationCodeStatus Status { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public int Attempts { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public virtual ApplicationUser? ApplicationUser { get; private set; }

        public bool IsExpired() {
            return DateTime.UtcNow > ExpiresAt;
        }

        public bool IsValid() {
            return !IsExpired() && Attempts < MaxAttempts && Status == VerificationCodeStatus.Active;
        }

        public void Revoke() {
            if (Status == VerificationCodeStatus.Active) {
                Status = VerificationCodeStatus.Revoked;
            }
        }

        public bool TryUse(string code) {
            if (!IsValid() || !string.Equals(Code, code, StringComparison.Ordinal)) {
                IncrementAttempts();
                return false;
            }

            Status = VerificationCodeStatus.Used;
            return true;
        }

        private void IncrementAttempts() {
            if (Status != VerificationCodeStatus.Active) {
                return;
            }

            Attempts++;

            if (Attempts >= MaxAttempts) {
                Status = VerificationCodeStatus.Revoked;
            }
        }
    }
}
