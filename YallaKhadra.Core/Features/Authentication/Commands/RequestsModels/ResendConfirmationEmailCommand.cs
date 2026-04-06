using MediatR;
using YallaKhadra.Core.Bases.Responses;

namespace YallaKhadra.Core.Features.Authentication.Commands.RequestsModels {
    public class ResendConfirmationEmailCommand : IRequest<Response> {
        public string Email { get; set; } = string.Empty;
    }
}
