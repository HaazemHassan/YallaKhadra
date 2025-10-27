using FluentValidation;
using YallaKhadra.Core.Features.Authentication.Commands.RequestsModels;

namespace YallaKhadra.Core.Features.Authentication.Commands.Validators {
    public class RegisterValidator : AbstractValidator<RegisterCommand> {

        public RegisterValidator() {
            ApplyValidaionRules();
        }
        public void ApplyValidaionRules() {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null")
                .MinimumLength(3)
                .MaximumLength(15)
                .Matches(@"^[\p{L}]+$").WithMessage("First name must contain only letters.");

            RuleFor(x => x.LastName)
               .NotEmpty().WithMessage("{PropertyName} can't be empty")
               .NotNull().WithMessage("{PropertyName} can't be null")
               .MinimumLength(3)
               .MaximumLength(15)
               .Matches(@"^[\p{L}]+$").WithMessage("Last name must contain only letters.");


            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null")
                .MinimumLength(3)
                .MaximumLength(15)
                .Matches(@"^[a-zA-Z0-9._]+$").WithMessage("{PropertyName} can only contain letters, numbers, dots, and underscores.");


            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null")
                .MaximumLength(35)
                .EmailAddress().WithMessage("Invalid email address format.")
                .Matches(@"@.+\..+").WithMessage("{PropertyName} must contain a valid domain.");


            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null")
                .MinimumLength(3).WithMessage("{PropertyName} must be at least of length 3");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null")
                .Equal(x => x.Password).WithMessage("Password does not match");

            RuleFor(x => x.PhoneNumber)
                .Matches(expression: @"^\+?[0-9]\d{1,14}$")
                .WithMessage("Phone number is not valid.");
        }
    }
}