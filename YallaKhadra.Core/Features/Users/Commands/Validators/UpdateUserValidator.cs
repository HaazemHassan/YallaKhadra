using FluentValidation;
using YallaKhadra.Core.Features.Users.Commands.RequestModels;

namespace YallaKhadra.Core.Features.Users.Commands.Validators {
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand> {
        public UpdateUserValidator() {
            ApplyValidationRules();
            ApplyCustomValidations();
        }

        public void ApplyValidationRules() {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");



            When(x => x.Address != null, () => {
                RuleFor(x => x.Address)
                    .MinimumLength(4).WithMessage("{PropertyName} must be at least 4 characters")
                    .MaximumLength(20).WithMessage("{PropertyName} must not exceed 20 characters");
            });


        }

        public void ApplyCustomValidations() {
            RuleFor(x => x)
               .Must(HaveAtLeastOneNonNullProperty)
               .WithMessage("Nothing to change. At least one property must be provided for update.");
        }

        private bool HaveAtLeastOneNonNullProperty(UpdateUserCommand command) {
            return command.FirstName != null ||
                   command.LastName != null ||
                   command.Address != null;
        }
    }
}