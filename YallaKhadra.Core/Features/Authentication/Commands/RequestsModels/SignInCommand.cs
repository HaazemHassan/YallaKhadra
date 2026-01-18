using MediatR;
using YallaKhadra.Core.Bases.Authentication;
using YallaKhadra.Core.Bases.Responses;

namespace YallaKhadra.Core.Features.Authentication.Commands.RequestsModels;

public class SignInCommand : IRequest<Response<AuthResult>> {
    public string Email { get; set; }
    public string Password { get; set; }

}
