using FluentValidation;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Users.Queries.Models;

namespace YallaKhadra.Core.Features.Users.Queries.Validators {
    public class GetUsersByRoleValidator : AbstractValidator<GetUsersByRoleQuery> {
        public GetUsersByRoleValidator() {
            RuleFor(x => x.Role)
                .Must(role => role == UserRole.Admin || role == UserRole.Worker || role == UserRole.User)
                .WithMessage("Role must be Admin, Worker, or User.");

            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .When(x => x.PageNumber.HasValue);

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .When(x => x.PageSize.HasValue);
        }
    }
}
