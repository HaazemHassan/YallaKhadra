using FluentValidation;
using YallaKhadra.Core.Features.Categories.Commands.RequestModels;

namespace YallaKhadra.Core.Features.Categories.Commands.Validators {
    public class AddCategoryCommandValidator : AbstractValidator<AddCategoryCommand> {
        public AddCategoryCommandValidator() {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required")
                .MaximumLength(50).WithMessage("Category name cannot exceed 50 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Category description is required")
                .MaximumLength(200).WithMessage("Category description cannot exceed 200 characters");
        }
    }
}
