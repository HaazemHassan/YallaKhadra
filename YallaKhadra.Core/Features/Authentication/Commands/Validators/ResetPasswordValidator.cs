using FluentValidation;
using YallaKhadra.Core.Features.Authentication.Commands.RequestsModels;

namespace YallaKhadra.Core.Features.Authentication.Commands.Validators {
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand> {
        public ResetPasswordValidator() {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null")
                .MaximumLength(100)
                .EmailAddress().WithMessage("Invalid email address format.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null")
                .MinimumLength(4)
                .MaximumLength(10);

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null")
                .MinimumLength(8).WithMessage("{PropertyName} must be at least of length 8");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null")
                .Equal(x => x.NewPassword).WithMessage("Password does not match");
        }
    }
}
