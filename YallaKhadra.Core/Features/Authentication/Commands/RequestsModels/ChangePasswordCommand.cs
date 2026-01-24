using MediatR;
using YallaKhadra.Core.Bases.Responses;

namespace YallaKhadra.Core.Features.Authentication.Commands.RequestsModels {
    public class ChangePasswordCommand : IRequest<Response> {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;

    }
}
