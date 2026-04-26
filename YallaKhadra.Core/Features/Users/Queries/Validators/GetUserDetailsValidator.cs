using FluentValidation;
using YallaKhadra.Core.Features.Users.Queries.Models;

namespace YallaKhadra.Core.Features.Users.Queries.Validators {
    public class GetUserDetailsValidator : AbstractValidator<GetUserDetailsQuery> {
        public GetUserDetailsValidator() {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("User id must be greater than 0.");
        }
    }
}
