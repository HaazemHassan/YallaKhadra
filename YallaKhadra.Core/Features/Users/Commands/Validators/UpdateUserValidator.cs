using System.IO;
using FluentValidation;
using YallaKhadra.Core.Features.Users.Commands.RequestModels;

namespace YallaKhadra.Core.Features.Users.Commands.Validators {
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand> {
        public UpdateUserValidator() {
            ApplyValidationRules();
            ApplyCustomValidations();
        }

        public void ApplyValidationRules() {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");



            When(x => x.Address != null, () => {
                RuleFor(x => x.Address)
                    .MinimumLength(4).WithMessage("{PropertyName} must be at least 4 characters")
                    .MaximumLength(20).WithMessage("{PropertyName} must not exceed 20 characters");
            });


            When(x => x.PhoneNumber != null, () => {
                RuleFor(x => x.PhoneNumber)
                    .Matches(expression: @"^\+?[0-9]\d{1,14}$")
                    .WithMessage("Phone number is not valid.");
            });

            When(x => x.ProfileImage != null, () => {
                RuleFor(x => x.ProfileImage)
                    .Must(file => file != null && file.Length > 0)
                    .WithMessage("Profile image must not be empty")
                    .Must(IsValidImageType)
                    .WithMessage("Only image files (jpg, jpeg, png, gif, webp) are allowed")
                    .Must(file => file == null || file.Length <= 5 * 1024 * 1024)
                    .WithMessage("Profile image size must not exceed 5MB");
            });
        }

        public void ApplyCustomValidations() {
            RuleFor(x => x)
               .Must(HaveAtLeastOneNonNullProperty)
               .WithMessage("Nothing to change. At least one property must be provided for update.");
        }

        private bool HaveAtLeastOneNonNullProperty(UpdateUserCommand command) {
            return command.FirstName != null ||
                   command.LastName != null ||
                   command.Address != null ||
                   command.PhoneNumber != null ||
                   command.ProfileImage != null;
        }

        private bool IsValidImageType(Microsoft.AspNetCore.Http.IFormFile file) {
            if (file == null) return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            return allowedExtensions.Contains(extension);
        }
    }
}