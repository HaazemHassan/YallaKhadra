using MediatR;
using YallaKhadra.Core.Bases.Authentication;
using YallaKhadra.Core.Bases.Responses;

namespace YallaKhadra.Core.Features.Users.Commands.RequestModels {
    public class RegisterCommand : IRequest<Response<AuthResult>> {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }

    }
}
