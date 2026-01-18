using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Bases.Authentication;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Authentication.Commands.RequestsModels;
using YallaKhadra.Core.Features.Users.Queries.Responses;

namespace YallaKhadra.Core.Features.Authentication.Commands.Handlers;

public class AuthenticationCommandHandler : ResponseHandler,
                                                         IRequestHandler<SignInCommand, Response<AuthResult>>,
                                                         IRequestHandler<RefreshTokenCommand, Response<AuthResult>>,
                                                         IRequestHandler<LogoutCommand, Response<bool>> {


    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationUserService _applicationUserService;
    private readonly IAuthenticationService _authenticationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;


    public AuthenticationCommandHandler(IApplicationUserService applicationUserService, UserManager<ApplicationUser> userManager, IMapper mapper, IAuthenticationService authenticationService, ICurrentUserService currentUserService, IUnitOfWork unitOfWork) {
        _applicationUserService = applicationUserService;
        _userManager = userManager;
        _mapper = mapper;
        _authenticationService = authenticationService;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }



    public async Task<Response<AuthResult>> Handle(SignInCommand request, CancellationToken cancellationToken) {
        try {
            var userFromDb = await _userManager.FindByEmailAsync(request.Email);
            if (userFromDb is null)
                return Unauthorized<AuthResult>("Invalid Email or password");

            bool isAuthenticated = await _userManager.CheckPasswordAsync(userFromDb, request.Password);
            if (!isAuthenticated)
                return Unauthorized<AuthResult>("Invalid Email or password");

            //if (!userFromDb.EmailConfirmed)
            //    return Unauthorized<JwtResult>("Please confirm your email first");

            var authResult = await _authenticationService.AuthenticateAsync(userFromDb);
            if (authResult.Status != ServiceOperationStatus.Succeeded || authResult.Data is null)
                return BadRequest<AuthResult>(authResult.ErrorMessage ?? "Something went wrong");

            authResult.Data.User = _mapper.Map<GetUserByIdResponse>(userFromDb);
            return Success(authResult.Data);
        }
        catch (Exception ex) {
            return BadRequest<AuthResult>($"An error occurred: {ex.Message}");
        }
    }

    public async Task<Response<AuthResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken) {
        try {
            var authResult = await _authenticationService.ReAuthenticateAsync(request.RefreshToken, request.AccessToken);

            if (authResult.Status != ServiceOperationStatus.Succeeded || authResult.Data is null) {
                return authResult.Status switch {
                    ServiceOperationStatus.Unauthorized => Unauthorized<AuthResult>(authResult.ErrorMessage ?? "Invalid token"),
                    _ => BadRequest<AuthResult>(authResult.ErrorMessage ?? "Something went wrong")
                };
            }

            var user = await _userManager.FindByIdAsync(authResult.Data.RefreshToken.UserId.ToString());

            authResult.Data.User = _mapper.Map<GetUserByIdResponse>(user);
            return Success(authResult.Data);
        }
        catch (Exception ex) {
            return BadRequest<AuthResult>($"An error occurred: {ex.Message}");
        }
    }

    public async Task<Response<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken) {
        ServiceOperationResult<bool> serviceResult = await _authenticationService.LogoutAsync(request.RefreshToken!);
        return serviceResult.Status switch {
            ServiceOperationStatus.Succeeded => Success(serviceResult.Data),
            ServiceOperationStatus.Unauthorized => Unauthorized<bool>(serviceResult.ErrorMessage ?? "Invalid token"),
            _ => BadRequest<bool>(serviceResult.ErrorMessage ?? "Something went wrong")

        };
    }
}
