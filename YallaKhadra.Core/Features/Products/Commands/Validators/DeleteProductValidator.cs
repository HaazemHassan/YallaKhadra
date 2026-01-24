using FluentValidation;
using YallaKhadra.Core.Features.Products.Commands.RequestModels;

namespace YallaKhadra.Core.Features.Products.Commands.Validators {
    public class DeleteProductValidator : AbstractValidator<DeleteProductCommand> {
        public DeleteProductValidator() {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Product ID must be greater than 0");
        }
    }
}
