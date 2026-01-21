using FluentValidation;
using YallaKhadra.Core.Features.AIWasteScans.Commands.RequestModels;

namespace YallaKhadra.Core.Features.AIWasteScans.Commands.Validators {
    public class CreateAIWasteScanValidator : AbstractValidator<CreateAIWasteScanCommand> {
        public CreateAIWasteScanValidator() {
            RuleFor(x => x.Image)
                .NotNull()
                .WithMessage("Image is required.");

            RuleFor(x => x.Image)
                .Must(image => image == null || image.Length > 0)
                .WithMessage("Image file cannot be empty.")
                .Must(image => image == null || IsValidImageType(image.ContentType))
                .WithMessage("Only image files (jpg, jpeg, png, gif) are allowed.");
        }

        private bool IsValidImageType(string contentType) {
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
            return allowedTypes.Contains(contentType.ToLower());
        }
    }
}
