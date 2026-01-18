using MediatR;
using YallaKhadra.Core.Bases.Responses;

namespace YallaKhadra.Core.Features.Authentication.Commands.RequestsModels {
    public class LogoutCommand : IRequest<Response<bool>> {
        public string? RefreshToken { get; set; }
    }
}
