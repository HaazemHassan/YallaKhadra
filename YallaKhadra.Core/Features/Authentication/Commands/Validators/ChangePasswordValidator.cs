using FluentValidation;
using YallaKhadra.Core.Features.Authentication.Commands.RequestsModels;

namespace YallaKhadra.Core.Features.Authentication.Commands.Validators {
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand> {
        public ChangePasswordValidator() {
            ApplyValidaionRules();
        }

        public void ApplyValidaionRules() {

            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} is required");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} is required")
                .MinimumLength(3).WithMessage("{PropertyName} must be at least of length 3");

            RuleFor(x => x.ConfirmNewPassword)
                .Equal(x => x.NewPassword).WithMessage("Password does not match");
        }
    }
}

