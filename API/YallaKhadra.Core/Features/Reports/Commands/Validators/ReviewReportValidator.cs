using FluentValidation;
using YallaKhadra.Core.Features.Reports.Commands.RequestsModels;

namespace YallaKhadra.Core.Features.Reports.Commands.Validators
{
    public class ReviewReportValidator : AbstractValidator<ReviewReportCommand>
    {
        public ReviewReportValidator()
        {
            ApplyValidationsRules();
        }

        private void ApplyValidationsRules()
        {
            RuleFor(x => x.ReportId)
                .GreaterThan(0).WithMessage("Report ID must be greater than 0");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }
    }
}
