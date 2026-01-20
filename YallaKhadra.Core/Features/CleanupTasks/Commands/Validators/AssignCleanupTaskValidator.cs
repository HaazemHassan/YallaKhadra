using FluentValidation;
using YallaKhadra.Core.Features.CleanupTasks.Commands.RequestModels;

namespace YallaKhadra.Core.Features.CleanupTasks.Commands.Validators {
    public class AssignCleanupTaskValidator : AbstractValidator<AssignCleanupTaskCommand> {
        public AssignCleanupTaskValidator() {
            RuleFor(x => x.ReportId)
                .GreaterThan(0)
                .WithMessage("Report ID is required.");
        }
    }
}
