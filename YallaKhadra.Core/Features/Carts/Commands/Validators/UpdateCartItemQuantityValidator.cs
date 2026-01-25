using FluentValidation;
using YallaKhadra.Core.Features.Carts.Commands.RequestModels;

namespace YallaKhadra.Core.Features.Carts.Commands.Validators {
    public class UpdateCartItemQuantityValidator : AbstractValidator<UpdateCartItemQuantityCommand> {
        public UpdateCartItemQuantityValidator() {
            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0");
        }
    }
}
