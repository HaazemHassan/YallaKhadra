using FluentValidation;
using YallaKhadra.Core.Features.Carts.Commands.RequestModels;

namespace YallaKhadra.Core.Features.Carts.Commands.Validators {
    public class AddToCartValidator : AbstractValidator<AddToCartCommand> {
        public AddToCartValidator() {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Product ID must be greater than 0");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1")
                .LessThanOrEqualTo(100).WithMessage("Quantity cannot exceed 100");
        }
    }
}
