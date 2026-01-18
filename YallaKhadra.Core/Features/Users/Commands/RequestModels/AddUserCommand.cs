using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Users.Commands.Responses;

namespace YallaKhadra.Core.Features.Users.Commands.RequestModels {
    public class AddUserCommand : IRequest<Response<AddUserResponse>> {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public UserRole? UserRole { get; set; } = Enums.UserRole.User;
    }
}
