using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Bases.Options;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;
using YallaKhadra.Infrastructure.BackgroundJobs.Jobs;

namespace YallaKhadra.Services.Services {
    public class EmailVerificationService : IEmailVerificationService {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVerificationCodeRepository _verificationCodeRepository;
        private readonly IEmailBodyBuilderService _emailBodyBuilderService;
        private readonly VerificationCodeOptions _emailOptions;

        public EmailVerificationService(
            UserManager<ApplicationUser> userManager,
            IVerificationCodeRepository verificationCodeRepository,
            IEmailBodyBuilderService emailBodyBuilderService,
            IOptions<VerificationCodeOptions> emailVerificationCodeOptions) {
            _userManager = userManager;
            _verificationCodeRepository = verificationCodeRepository;
            _emailBodyBuilderService = emailBodyBuilderService;
            _emailOptions = emailVerificationCodeOptions.Value;
        }

        public async Task<ServiceOperationResult<string?>> CreateEmailConfirmationCodeAsync(int applicationUserId) {
            var appUser = await _userManager.FindByIdAsync(applicationUserId.ToString());

            if (appUser is null) {
                return ServiceOperationResult<string?>.Failure(ServiceOperationStatus.NotFound, "User not found.");
            }

            if (appUser.EmailConfirmed) {
                return ServiceOperationResult<string?>.Failure(ServiceOperationStatus.Failed, "Email is already confirmed.");
            }

            var existingCode = await _verificationCodeRepository.GetAsync(v =>
                v.ApplicationUserId == appUser.Id &&
                v.Type == VerificationCodeType.EmailConfirmation &&
                v.Status == VerificationCodeStatus.Active);

            if (existingCode is not null) {
                var elapsedSinceCreation = DateTime.UtcNow - existingCode.CreatedAt;

                if (elapsedSinceCreation < TimeSpan.FromMinutes(1)) {
                    return ServiceOperationResult<string?>.Failure(ServiceOperationStatus.Failed, "Please retry after a minute.");
                }

                existingCode.Revoke();
                await _verificationCodeRepository.UpdateAsync(existingCode);
            }

            var codeLength = _emailOptions.CodeLength > 0 ? _emailOptions.CodeLength : 6;
            var confirmEmailCode = GenerateOtp(codeLength);
            var codeExpiresAt = DateTime.UtcNow.AddMinutes(_emailOptions.EmailExpireInMinutes);

            var verificationCode = new VerificationCode(
                appUser.Id,
                confirmEmailCode,
                VerificationCodeType.EmailConfirmation,
                codeExpiresAt);

            await _verificationCodeRepository.AddAsync(verificationCode);
            return ServiceOperationResult<string?>.Success(confirmEmailCode);
        }

        public Task SendConfirmationEmailAsync(ApplicationUser user, string code) {
            if (string.IsNullOrWhiteSpace(user.Email)) {
                return Task.FromResult(ServiceOperationResult.Failure(ServiceOperationStatus.InvalidParameters, "User email is required."));
            }

            var fullName = string.IsNullOrWhiteSpace(user.FirstName)
                ? user.UserName ?? "User"
                : $"{user.FirstName} {user.LastName}".Trim();

            string emailBody = _emailBodyBuilderService.GenerateEmailBody("EmailConfirmation",
                new Dictionary<string, string> {
                    { "Name", fullName },
                    { "Code", code },
                    { "Minutes", _emailOptions.EmailExpireInMinutes.ToString() }
                });

            BackgroundJob.Enqueue<SendEmailJob>(job => job.Execute(user.Email, "Email Confirmation", emailBody));

            return Task.FromResult(ServiceOperationResult.Success());
        }

        public async Task<ServiceOperationResult> ConfirmEmailAsync(int applicationUserId, string code) {
            var appUser = await _userManager.FindByIdAsync(applicationUserId.ToString());

            if (appUser is null) {
                return ServiceOperationResult.Failure(ServiceOperationStatus.NotFound, "User not found.");
            }

            if (appUser.EmailConfirmed) {
                return ServiceOperationResult.Failure(ServiceOperationStatus.Failed, "Email is already confirmed.");
            }

            var verificationCode = await _verificationCodeRepository.GetAsync(v =>
                v.ApplicationUserId == appUser.Id &&
                v.Type == VerificationCodeType.EmailConfirmation &&
                v.Status == VerificationCodeStatus.Active);

            if (verificationCode is null) {
                return ServiceOperationResult.Failure(ServiceOperationStatus.NotFound, "Invalid code.");
            }

            var isCodeValid = verificationCode.TryUse(code);
            await _verificationCodeRepository.UpdateAsync(verificationCode);

            if (!isCodeValid) {
                return ServiceOperationResult.Failure(ServiceOperationStatus.Failed, "Invalid code.");
            }

            appUser.EmailConfirmed = true;
            var updateResult = await _userManager.UpdateAsync(appUser);

            if (!updateResult.Succeeded) {
                return ServiceOperationResult.Failure(
                    ServiceOperationStatus.Failed,
                    updateResult.Errors.FirstOrDefault()?.Description ?? "Failed to confirm email.");
            }

            return ServiceOperationResult.Success();
        }

        private static string GenerateOtp(int length) {
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
