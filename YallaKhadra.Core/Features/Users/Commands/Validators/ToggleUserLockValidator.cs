using FluentValidation;
using YallaKhadra.Core.Features.Users.Commands.RequestModels;

namespace YallaKhadra.Core.Features.Users.Commands.Validators {
    public class ToggleUserLockValidator : AbstractValidator<ToggleUserLockCommand> {
        public ToggleUserLockValidator() {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("User id must be greater than 0.");
        }
    }
}
