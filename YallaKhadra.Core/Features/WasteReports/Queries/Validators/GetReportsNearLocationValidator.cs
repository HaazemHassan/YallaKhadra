using FluentValidation;
using YallaKhadra.Core.Features.WasteReports.Queries.Models;

namespace YallaKhadra.Core.Features.WasteReports.Queries.Validators {
    public class GetReportsNearLocationValidator : AbstractValidator<GetReportsNearLocationQuery> {
        public GetReportsNearLocationValidator() {
            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180.");

            RuleFor(x => x.RadiusInKm)
                .GreaterThan(0)
                .WithMessage("Radius must be greater than 0.")
                .LessThanOrEqualTo(100)
                .WithMessage("Radius must not exceed 100 km.");
        }
    }
}
