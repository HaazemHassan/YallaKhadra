using FluentValidation;
using YallaKhadra.Core.Features.Products.Commands.RequestModels;

namespace YallaKhadra.Core.Features.Products.Commands.Validators {
    public class UpdateProductValidator : AbstractValidator<UpdateProductCommand> {
        public UpdateProductValidator() {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Product ID must be greater than 0");

            When(x => x.Name != null, () => {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Product name cannot be empty")
                    .MaximumLength(50).WithMessage("Product name cannot exceed 50 characters");
            });

            When(x => x.Description != null, () => {
                RuleFor(x => x.Description)
                    .NotEmpty().WithMessage("Product description cannot be empty")
                    .MaximumLength(200).WithMessage("Product description cannot exceed 200 characters");
            });

            When(x => x.PointsCost.HasValue, () => {
                RuleFor(x => x.PointsCost!.Value)
                    .GreaterThan(0).WithMessage("Points cost must be greater than 0");
            });

            When(x => x.Stock.HasValue, () => {
                RuleFor(x => x.Stock!.Value)
                    .GreaterThanOrEqualTo(0).WithMessage("Stock must be greater than or equal to 0");
            });

            When(x => x.Images != null && x.Images.Count > 0, () => {
                RuleFor(x => x.Images)
                    .Must(images => images!.Count == 3)
                    .WithMessage("If updating images, exactly 3 images are required (1 main image and 2 additional images)");

                RuleForEach(x => x.Images)
                    .Must(file => file != null && file.Length > 0)
                    .WithMessage("Each image file must not be empty")
                    .Must(IsValidImageType)
                    .WithMessage("Only image files (jpg, jpeg, png, gif, webp) are allowed")
                    .Must(file => file == null || file.Length <= 5 * 1024 * 1024)
                    .WithMessage("Each image file size must not exceed 5MB");
            });
        }

        private bool IsValidImageType(Microsoft.AspNetCore.Http.IFormFile file) {
            if (file == null) return false;
            
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            return allowedExtensions.Contains(extension);
        }
    }
}
