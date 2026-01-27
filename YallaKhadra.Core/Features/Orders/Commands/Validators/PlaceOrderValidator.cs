using FluentValidation;
using System.Text.RegularExpressions;
using YallaKhadra.Core.Features.Orders.Commands.RequestModels;

namespace YallaKhadra.Core.Features.Orders.Commands.Validators {
    public class PlaceOrderValidator : AbstractValidator<PlaceOrderCommand> {
        public PlaceOrderValidator() {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters")
                .MinimumLength(3).WithMessage("Full name must be at least 3 characters")
                .Matches(@"^[\u0600-\u06FFa-zA-Z\s]+$").WithMessage("Full name can only contain Arabic or English letters");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
                .Matches(@"^(010|011|012|015)\d{8}$")
                .WithMessage("Phone number must be a valid Egyptian mobile number (e.g., 01012345678)");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required")
                .MaximumLength(50).WithMessage("City name cannot exceed 50 characters")
                .Matches(@"^[\u0600-\u06FFa-zA-Z\s]+$").WithMessage("City name can only contain Arabic or English letters");

            RuleFor(x => x.StreetAddress)
                .NotEmpty().WithMessage("Street address is required")
                .MaximumLength(200).WithMessage("Street address cannot exceed 200 characters")
                .MinimumLength(5).WithMessage("Street address must be at least 5 characters");

            When(x => !string.IsNullOrWhiteSpace(x.BuildingNumber), () => {
                RuleFor(x => x.BuildingNumber)
                    .MaximumLength(20).WithMessage("Building number cannot exceed 20 characters");
            });

            When(x => !string.IsNullOrWhiteSpace(x.Landmark), () => {
                RuleFor(x => x.Landmark)
                    .MaximumLength(100).WithMessage("Landmark cannot exceed 100 characters");
            });

            When(x => !string.IsNullOrWhiteSpace(x.ShippingNotes), () => {
                RuleFor(x => x.ShippingNotes)
                    .MaximumLength(500).WithMessage("Shipping notes cannot exceed 500 characters");
            });
        }
    }
}
