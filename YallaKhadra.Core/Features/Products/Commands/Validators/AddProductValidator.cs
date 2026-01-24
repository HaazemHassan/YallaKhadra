using FluentValidation;
using YallaKhadra.Core.Features.Products.Commands.RequestModels;

namespace YallaKhadra.Core.Features.Products.Commands.Validators {
    public class AddProductValidator : AbstractValidator<AddProductCommand> {
        public AddProductValidator() {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(50).WithMessage("Product name cannot exceed 50 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Product description is required")
                .MaximumLength(200).WithMessage("Product description cannot exceed 200 characters");

            RuleFor(x => x.PointsCost)
                .GreaterThan(0).WithMessage("Points cost must be greater than 0");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Stock must be greater than or equal to 0");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category ID must be greater than 0");

            RuleFor(x => x.Images)
                .NotNull().WithMessage("Images are required")
                .Must(images => images != null && images.Count == 3)
                .WithMessage("Exactly 3 images are required (1 main image and 2 additional images)");

            RuleForEach(x => x.Images)
                .Must(file => file != null && file.Length > 0)
                .WithMessage("Each image file must not be empty")
                .Must(IsValidImageType)
                .WithMessage("Only image files (jpg, jpeg, png, gif, webp) are allowed")
                .Must(file => file == null || file.Length <= 5 * 1024 * 1024)
                .WithMessage("Each image file size must not exceed 5MB");
        }

        private bool IsValidImageType(Microsoft.AspNetCore.Http.IFormFile file) {
            if (file == null) return false;
            
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            return allowedExtensions.Contains(extension);
        }
    }
}
