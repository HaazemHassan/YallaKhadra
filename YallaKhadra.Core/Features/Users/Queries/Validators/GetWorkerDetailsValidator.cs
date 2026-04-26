using FluentValidation;
using YallaKhadra.Core.Features.Users.Queries.Models;

namespace YallaKhadra.Core.Features.Users.Queries.Validators {
    public class GetWorkerDetailsValidator : AbstractValidator<GetWorkerDetailsQuery> {
        public GetWorkerDetailsValidator() {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Worker id must be greater than 0.");
        }
    }
}
