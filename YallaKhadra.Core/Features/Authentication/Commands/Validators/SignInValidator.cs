using FluentValidation;
using YallaKhadra.Core.Features.Authentication.Commands.RequestsModels;

namespace YallaKhadra.Core.Features.Authentication.Commands.Validators {
    public class SignInValidator : AbstractValidator<SignInCommand> {
        public SignInValidator() {
            ApplyValidaionRules();
        }
        public void ApplyValidaionRules() {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} is required");


            RuleFor(x => x.Password)
             .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} is required");

        }

    }
}