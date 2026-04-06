using FluentValidation;
using YallaKhadra.Core.Features.Authentication.Queries.RequestsModels;

namespace YallaKhadra.Core.Features.Authentication.Queries.Validators {
    public class ValidateResetPasswordCodeValidator : AbstractValidator<ValidateResetPasswordCodeQuery> {
        public ValidateResetPasswordCodeValidator() {
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
        }
    }
}
