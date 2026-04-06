using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Authentication.Queries.Responses;

namespace YallaKhadra.Core.Features.Authentication.Queries.RequestsModels {
    public class ValidateResetPasswordCodeQuery : IRequest<Response<ValidateResetPasswordCodeResponse>> {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
