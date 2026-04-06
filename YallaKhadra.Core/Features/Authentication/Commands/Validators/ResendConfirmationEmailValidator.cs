using FluentValidation;
using YallaKhadra.Core.Features.Authentication.Commands.RequestsModels;

namespace YallaKhadra.Core.Features.Authentication.Commands.Validators {
    public class ResendConfirmationEmailValidator : AbstractValidator<ResendConfirmationEmailCommand> {
        public ResendConfirmationEmailValidator() {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null")
                .MaximumLength(100)
                .EmailAddress().WithMessage("Invalid email address format.");
        }
    }
}
