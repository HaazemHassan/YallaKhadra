using MediatR;
using YallaKhadra.Core.Bases.Authentication;
using YallaKhadra.Core.Bases.Responses;

namespace YallaKhadra.Core.Features.Authentication.Commands.RequestsModels;

public class GoogleLoginCommand : IRequest<Response<AuthResult>> {
    public string IdToken { get; set; } = string.Empty;
}
