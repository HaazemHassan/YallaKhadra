using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Bases.Options;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;
using YallaKhadra.Infrastructure.BackgroundJobs.Jobs;

namespace YallaKhadra.Services.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVerificationCodeRepository _verificationCodeRepository;
        private readonly IOtpService _otpService;
        private readonly IEmailBodyBuilderService _emailBodyBuilderService;
        private readonly VerificationCodeOptions _emailOptions;

        public PasswordService(
            UserManager<ApplicationUser> userManager,
            IVerificationCodeRepository verificationCodeRepository,
            IOtpService otpService,
            IEmailBodyBuilderService emailBodyBuilderService,
            IOptions<VerificationCodeOptions> verificationCodeOptions)
        {
            _userManager = userManager;
            _verificationCodeRepository = verificationCodeRepository;
            _otpService = otpService;
            _emailBodyBuilderService = emailBodyBuilderService;
            _emailOptions = verificationCodeOptions.Value;
        }

        public async Task<ServiceOperationResult<string?>> CreatePasswordResetCodeAsync(int applicationUserId)
        {
            var appUser = await _userManager.FindByIdAsync(applicationUserId.ToString());

            if (appUser is null)
            {
                return ServiceOperationResult<string>.Failure(ServiceOperationStatus.NotFound, "User not found.");
            }

            var existingCode = await _verificationCodeRepository.GetAsync(v =>
                v.ApplicationUserId == appUser.Id &&
                v.Type == VerificationCodeType.PasswordReset &&
                v.Status == VerificationCodeStatus.Active);

            if (existingCode is not null)
            {
                existingCode.Revoke();
                await _verificationCodeRepository.UpdateAsync(existingCode);
            }

            var codeLength = _emailOptions.CodeLength > 0 ? _emailOptions.CodeLength : 6;
            var resetPasswordCode = _otpService.Generate(codeLength);
            var codeExpiresAt = DateTime.UtcNow.AddMinutes(_emailOptions.EmailExpireInMinutes);

            var verificationCode = new VerificationCode(
                appUser.Id,
                resetPasswordCode,
                VerificationCodeType.PasswordReset,
                codeExpiresAt);

            await _verificationCodeRepository.AddAsync(verificationCode);

            return ServiceOperationResult<string>.Success(resetPasswordCode);
        }

        public Task SendPasswordResetEmailAsync(ApplicationUser user, string code)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return Task.FromResult(ServiceOperationResult.Failure(ServiceOperationStatus.InvalidParameters, "User email is required."));
            }

            var fullName = string.IsNullOrWhiteSpace(user.FirstName)
                ? user.UserName ?? "User"
                : $"{user.FirstName} {user.LastName}".Trim();

            string emailBody = _emailBodyBuilderService.GenerateEmailBody("PasswordReset",
                new Dictionary<string, string> {
                    { "Name", fullName },
                    { "Code", code },
                    { "Minutes", _emailOptions.EmailExpireInMinutes.ToString() }
                });

            BackgroundJob.Enqueue<SendEmailJob>(job => job.Execute(user.Email, "Password Reset", emailBody));

            return Task.FromResult(ServiceOperationResult.Success());
        }

        public async Task<ServiceOperationResult<bool>> IsPasswordResetCodeValidAsync(string email, string code)
        {
            var appUser = await _userManager.FindByEmailAsync(email);

            if (appUser is null)
            {
                return ServiceOperationResult<bool>.Success(false);
            }

            var verificationCode = await _verificationCodeRepository.GetAsync(v =>
                v.ApplicationUserId == appUser.Id &&
                v.Type == VerificationCodeType.PasswordReset &&
                v.Status == VerificationCodeStatus.Active &&
                v.Code == code);

            if (verificationCode is null)
            {
                return ServiceOperationResult<bool>.Success(false);
            }

            return ServiceOperationResult<bool>.Success(verificationCode.IsValid());
        }

        public async Task<ServiceOperationResult> ResetPasswordAsync(string email, string code, string newPassword)
        {
            var appUser = await _userManager.FindByEmailAsync(email);

            if (appUser is null)
            {
                return ServiceOperationResult.Failure(ServiceOperationStatus.NotFound, "User not found.");
            }

            var verificationCode = await _verificationCodeRepository.GetAsync(v =>
                v.ApplicationUserId == appUser.Id &&
                v.Type == VerificationCodeType.PasswordReset &&
                v.Status == VerificationCodeStatus.Active);

            if (verificationCode is null)
            {
                return ServiceOperationResult.Failure(ServiceOperationStatus.NotFound, "Invalid code.");
            }

            var isCodeValid = verificationCode.TryUse(code);

            await _verificationCodeRepository.UpdateAsync(verificationCode);

            if (!isCodeValid)
            {
                return ServiceOperationResult.Failure(ServiceOperationStatus.Failed, "Invalid code.");
            }

            var identityResetToken = await _userManager.GeneratePasswordResetTokenAsync(appUser);
            var resetResult = await _userManager.ResetPasswordAsync(appUser, identityResetToken, newPassword);

            if (!resetResult.Succeeded)
            {
                return ServiceOperationResult.Failure(
                    ServiceOperationStatus.Failed,
                    resetResult.Errors.FirstOrDefault()?.Description ?? "Password reset failed.");
            }

            return ServiceOperationResult.Success();
        }
    }
}
