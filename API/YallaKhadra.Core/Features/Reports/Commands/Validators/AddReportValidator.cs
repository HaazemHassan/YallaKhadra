using FluentValidation;
using Microsoft.AspNetCore.Http;
using YallaKhadra.Core.Features.Reports.Commands.RequestsModels;

namespace YallaKhadra.Core.Features.Reports.Commands.Validators
{
    public class AddReportValidator : AbstractValidator<AddReportCommand>
    {
        #region Constructors
        public AddReportValidator()
        {
            ApplyValidationsRules();
            ApplyCustomValidationsRules();
        }
        #endregion

        #region Functions

        private void ApplyValidationsRules()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description must not be empty")
                .NotNull().WithMessage("Description must not be null")
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters");


            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90");


            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address must not be empty")
                .NotNull().WithMessage("Address must not be null")
                .MaximumLength(200).WithMessage("Address must not exceed 200 characters");

            RuleFor(x => x.Photos)
                .NotNull().WithMessage("At least one photo is required")
                .Must(photos => photos == null || photos.Count > 0)
                    .WithMessage("You must upload at least one photo")
                .Must(photos => photos == null || photos.All(IsValidImage))
                    .WithMessage("One or more uploaded files are not valid image types (jpg, jpeg, png)");

            RuleForEach(x => x.Photos)
                .Must(photo => photo.Length <= 5 * 1024 * 1024)
                .WithMessage("Each photo must not exceed 5MB in size");
        }

        private void ApplyCustomValidationsRules()
        {

        }

        #endregion

        #region Helpers
        private bool IsValidImage(IFormFile file)
        {
            if (file == null) return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            return allowedExtensions.Contains(extension);
        }
        #endregion
    }
}
