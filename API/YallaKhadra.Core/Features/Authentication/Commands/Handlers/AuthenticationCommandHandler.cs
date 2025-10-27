using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Authentication;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Authentication.Commands.RequestsModels;
using YallaKhadra.Core.Features.Users.Queries.Responses;

namespace YallaKhadra.Core.Features.Authentication.Commands.Handlers;

public class AuthenticationCommandHandler : ResponseHandler, IRequestHandler<RegisterCommand, Response<JwtResult>>,
                                                         IRequestHandler<SignInCommand, Response<JwtResult>>,
                                                         IRequestHandler<RefreshTokenCommand, Response<JwtResult>>,
                                                         IRequestHandler<GoogleLoginCommand, Response<JwtResult>> {


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


    public async Task<Response<JwtResult>> Handle(RegisterCommand request, CancellationToken cancellationToken) {
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try {
            var userMapped = _mapper.Map<ApplicationUser>(request);
            var addUserResult = await _applicationUserService.AddUser(userMapped, request.Password);

            if (addUserResult.Status != ServiceOperationStatus.Succeeded || addUserResult.Data is null) {
                await _unitOfWork.RollbackAsync();
                return addUserResult.Status switch {
                    ServiceOperationStatus.AlreadyExists => Conflict<JwtResult>(addUserResult.ErrorMessage),
                    _ => BadRequest<JwtResult>(addUserResult.ErrorMessage),
                };
            }

            var user = addUserResult.Data;
            var authResult = await _authenticationService.AuthenticateAsync(user);
            if (authResult.Status != ServiceOperationStatus.Succeeded || authResult.Data is null) {
                await _unitOfWork.RollbackAsync();
                return BadRequest<JwtResult>(authResult.ErrorMessage);
            }

            authResult.Data.User = _mapper.Map<GetUserByIdResponse>(user);

            await _unitOfWork.CommitAsync();
            return Created(authResult.Data);
        }
        catch (Exception ex) {
            await _unitOfWork.RollbackAsync();
            return BadRequest<JwtResult>($"An error occurred: {ex.Message}");
        }
    }

    public async Task<Response<JwtResult>> Handle(SignInCommand request, CancellationToken cancellationToken) {
        try {
            var userFromDb = await _userManager.FindByNameAsync(request.Username);
            if (userFromDb is null)
                return Unauthorized<JwtResult>("Invalid username or password");

            bool isAuthenticated = await _userManager.CheckPasswordAsync(userFromDb, request.Password);
            if (!isAuthenticated)
                return Unauthorized<JwtResult>("Invalid username or password");

            //if (!userFromDb.EmailConfirmed)
            //    return Unauthorized<JwtResult>("Please confirm your email first");

            var authResult = await _authenticationService.AuthenticateAsync(userFromDb);
            if (authResult.Status != ServiceOperationStatus.Succeeded || authResult.Data is null)
                return BadRequest<JwtResult>(authResult.ErrorMessage ?? "Something went wrong");

            authResult.Data.User = _mapper.Map<GetUserByIdResponse>(userFromDb);
            return Success(authResult.Data);
        }
        catch (Exception ex) {
            return BadRequest<JwtResult>($"An error occurred: {ex.Message}");
        }
    }

    public async Task<Response<JwtResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken) {
        try {
            var authResult = await _authenticationService.ReAuthenticateAsync(request.RefreshToken, request.AccessToken);

            if (authResult.Status != ServiceOperationStatus.Succeeded || authResult.Data is null) {
                return authResult.Status switch {
                    ServiceOperationStatus.Unauthorized => Unauthorized<JwtResult>(authResult.ErrorMessage ?? "Invalid token"),
                    _ => BadRequest<JwtResult>(authResult.ErrorMessage ?? "Something went wrong")
                };
            }

            var user = await _userManager.FindByIdAsync(authResult.Data.RefreshToken.UserId.ToString());

            authResult.Data.User = _mapper.Map<GetUserByIdResponse>(user);
            return Success(authResult.Data);
        }
        catch (Exception ex) {
            return BadRequest<JwtResult>($"An error occurred: {ex.Message}");
        }
    }

    public async Task<Response<JwtResult>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken) {
        try {
            var serviceResult = await _authenticationService.GoogleAuthenticateAsync(request.IdToken);

            if (serviceResult.Status != ServiceOperationStatus.Succeeded || serviceResult.Data is null) {
                return serviceResult.Status switch {
                    ServiceOperationStatus.Unauthorized => Unauthorized<JwtResult>(serviceResult.ErrorMessage ?? "Invalid Google token"),
                    ServiceOperationStatus.InvalidParameters => BadRequest<JwtResult>(serviceResult.ErrorMessage ?? "Invalid parameters"),
                    ServiceOperationStatus.Failed => BadRequest<JwtResult>(serviceResult.ErrorMessage ?? "Authentication failed"),
                    _ => BadRequest<JwtResult>("An unexpected error occurred")
                };
            }

            return Success(serviceResult.Data);
        }
        catch (Exception ex) {
            return BadRequest<JwtResult>($"An error occurred: {ex.Message}");
        }
    }
}
