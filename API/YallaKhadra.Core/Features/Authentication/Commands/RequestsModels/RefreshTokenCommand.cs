using MediatR;
using YallaKhadra.Core.Bases.Authentication;
using YallaKhadra.Core.Bases.Responses;

namespace YallaKhadra.Core.Features.Authentication.Commands.RequestsModels;

public class RefreshTokenCommand : IRequest<Response<AuthResult>> {
    public string AccessToken { get; set; }
    public string? RefreshToken { get; set; }

}
