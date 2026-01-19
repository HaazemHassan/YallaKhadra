using FluentValidation;
using YallaKhadra.Core.Features.WasteReports.Commands.RequestModels;

namespace YallaKhadra.Core.Features.WasteReports.Commands.Validators {
    public class CreateWasteReportValidator : AbstractValidator<CreateWasteReportCommand> {
        public CreateWasteReportValidator() {
            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180.");

            RuleFor(x => x.WasteType)
                .GreaterThan(0)
                .WithMessage("Waste type is required.");

            RuleFor(x => x.Address)
                .MaximumLength(500)
                .WithMessage("Address must not exceed 500 characters.");

            RuleFor(x => x.Images)
                .Must(images => images == null || images.Count <= 10)
                .WithMessage("Maximum 10 images allowed per report.");

            RuleForEach(x => x.Images)
                .Must(image => image == null || image.Length <= 10 * 1024 * 1024) // 10MB per image
                .WithMessage("Each image must not exceed 10MB.")
                .Must(image => image == null || IsValidImageType(image.ContentType))
                .WithMessage("Only image files (jpg, jpeg, png, gif) are allowed.");
        }

        private bool IsValidImageType(string contentType) {
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
            return allowedTypes.Contains(contentType.ToLower());
        }
    }
}
