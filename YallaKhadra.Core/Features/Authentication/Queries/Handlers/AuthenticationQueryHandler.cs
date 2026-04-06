using MediatR;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Authentication.Queries.RequestsModels;
using YallaKhadra.Core.Features.Authentication.Queries.Responses;

namespace YallaKhadra.Core.Features.Authentication.Queries.Handlers
{
    public class AuthenticationQueryHandler : ResponseHandler,
        IRequestHandler<ValidateResetPasswordCodeQuery, Response<ValidateResetPasswordCodeResponse>>
    {
        private readonly IPasswordService _passwordService;

        public AuthenticationQueryHandler(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        public async Task<Response<ValidateResetPasswordCodeResponse>> Handle(ValidateResetPasswordCodeQuery request, CancellationToken cancellationToken)
        {
            var isValidResult = await _passwordService.IsPasswordResetCodeValidAsync(request.Email, request.Code);

            if (isValidResult.Status != ServiceOperationStatus.Succeeded)
            {
                return BadRequest<ValidateResetPasswordCodeResponse>(isValidResult.ErrorMessage ?? "Failed to validate code.");
            }

            return Success(new ValidateResetPasswordCodeResponse {
                IsValid = isValidResult.Data
            });
        }
    }
}
