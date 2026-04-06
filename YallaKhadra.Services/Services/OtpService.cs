using System.Security.Cryptography;
using YallaKhadra.Core.Abstracts.ServicesContracts;

namespace YallaKhadra.Services.Services {
    public class OtpService : IOtpService {
        public string Generate(int length = 6) {
            if (length <= 0) {
                throw new ArgumentException("OTP length must be greater than zero.");
            }

            var digits = new char[length];

            for (int i = 0; i < length; i++) {
                digits[i] = (char)('0' + RandomNumberGenerator.GetInt32(0, 10));
            }

            return new string(digits);
        }
    }
}
