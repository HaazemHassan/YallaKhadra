using FluentValidation;
using YallaKhadra.Core.Features.CleanupTasks.Commands.RequestModels;

namespace YallaKhadra.Core.Features.CleanupTasks.Commands.Validators {
    public class CompleteCleanupTaskValidator : AbstractValidator<CompleteCleanupTaskCommand> {
        public CompleteCleanupTaskValidator() {
            RuleFor(x => x.TaskId)
                .GreaterThan(0)
                .WithMessage("Task ID is required.");

            RuleFor(x => x.FinalWeightInKg)
                .GreaterThan(0)
                .WithMessage("Weight must be greater than zero.")
                .LessThanOrEqualTo(10000)
                .WithMessage("Weight must not exceed 10,000 kg.");

            RuleFor(x => x.FinalWasteType)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Waste type is required.");

            RuleFor(x => x.Images)
                .NotEmpty()
                .WithMessage("At least one image is required.")
                .Must(images => images == null || images.Count <= 10)
                .WithMessage("Maximum 10 images allowed per task.");

            RuleForEach(x => x.Images)
                .Must(image => image == null || IsValidImageType(image.ContentType))
                .WithMessage("Only image files (jpg, jpeg, png, gif) are allowed.");
        }

        private bool IsValidImageType(string contentType) {
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
            return allowedTypes.Contains(contentType.ToLower());
        }
    }
}
