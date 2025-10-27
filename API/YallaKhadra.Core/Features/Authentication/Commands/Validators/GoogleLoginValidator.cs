using FluentValidation;
using YallaKhadra.Core.Features.Authentication.Commands.RequestsModels;

namespace YallaKhadra.Core.Features.Authentication.Commands.Validators {
    public class GoogleLoginValidator : AbstractValidator<GoogleLoginCommand> {
        public GoogleLoginValidator() {
            RuleFor(x => x.IdToken)
                .NotEmpty().WithMessage("Google ID Token is required")
                .NotNull().WithMessage("Google ID Token cannot be null");
        }
    }
}
