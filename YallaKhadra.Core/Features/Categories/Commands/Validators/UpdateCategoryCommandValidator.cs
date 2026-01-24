using FluentValidation;
using YallaKhadra.Core.Features.Categories.Commands.RequestModels;

namespace YallaKhadra.Core.Features.Categories.Commands.Validators {
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand> {
        public UpdateCategoryCommandValidator() {

            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.Name) || !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage("At least one field (Name or Description) must be provided for update");

            When(x => !string.IsNullOrWhiteSpace(x.Name), () => {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Category name cannot be empty")
                    .MaximumLength(50).WithMessage("Category name cannot exceed 50 characters");
            });

            When(x => !string.IsNullOrWhiteSpace(x.Description), () => {
                RuleFor(x => x.Description)
                    .NotEmpty().WithMessage("Category description cannot be empty")
                    .MaximumLength(200).WithMessage("Category description cannot exceed 200 characters");
            });
        }
    }
}
