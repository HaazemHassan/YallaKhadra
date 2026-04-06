using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
                                                         IRequestHandler<LogoutCommand, Response<bool>>,
                                                         IRequestHandler<ChangePasswordCommand, Response>,
                                                         IRequestHandler<ResendConfirmationEmailCommand, Response>,
                                                         IRequestHandler<ConfirmEmailCommand, Response> {


    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthenticationService _authenticationService;
    private readonly IEmailVerificationService _emailVerificationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;


    public AuthenticationCommandHandler(UserManager<ApplicationUser> userManager, IMapper mapper, IAuthenticationService authenticationService, IEmailVerificationService emailVerificationService, ICurrentUserService currentUserService, IUnitOfWork unitOfWork) {
        _userManager = userManager;
        _mapper = mapper;
        _authenticationService = authenticationService;
        _emailVerificationService = emailVerificationService;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }



    public async Task<Response<AuthResult>> Handle(SignInCommand request, CancellationToken cancellationToken) {
        try {
            var userFromDb = await _userManager.Users
                .Include(u => u.ProfileImage)
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (userFromDb is null)
                return Unauthorized<AuthResult>("Invalid Email or password");

            bool isAuthenticated = await _userManager.CheckPasswordAsync(userFromDb, request.Password);
            if (!isAuthenticated)
                return Unauthorized<AuthResult>("Invalid Email or password");

            if (!userFromDb.EmailConfirmed)
                return Unauthorized<AuthResult>("Please confirm your email first", ErrorCodes.Authentication.EmailNotConfirmed);

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
            var authResult = await _authenticationService.ReAuthenticateAsync(request.RefreshToken!, request.AccessToken);

            if (authResult.Status != ServiceOperationStatus.Succeeded || authResult.Data is null) {
                return authResult.Status switch {
                    ServiceOperationStatus.Unauthorized => Unauthorized<AuthResult>(authResult.ErrorMessage ?? "Invalid token"),
                    _ => BadRequest<AuthResult>(authResult.ErrorMessage ?? "Something went wrong")
                };
            }

            var user = await _userManager.Users
                .Include(u => u.ProfileImage)
                .FirstOrDefaultAsync(u => u.Id == authResult.Data.RefreshToken!.UserId, cancellationToken);

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

    public async Task<Response> Handle(ChangePasswordCommand request, CancellationToken cancellationToken) {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized("User not found !");


        var userFromDb = await _userManager.FindByIdAsync(userId.Value.ToString());
        if (userFromDb is null)
            return NotFound();

        bool isUserPasswordCorrect = await _userManager.CheckPasswordAsync(userFromDb, request.CurrentPassword);
        if (!isUserPasswordCorrect)
            return Unauthorized("Current password is invalid.");

        var result = await _userManager.ChangePasswordAsync(userFromDb, request.CurrentPassword, request.NewPassword);
        if (result.Succeeded)
            return Updated("Password updated successfully");

        return BadRequest("Something went wrong while updating.");
    }

    public async Task<Response> Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken) {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null) {
            return Success();
        }

        var createCodeResult = await _emailVerificationService.CreateEmailConfirmationCodeAsync(user.Id);

        if (createCodeResult.Status != ServiceOperationStatus.Succeeded || string.IsNullOrWhiteSpace(createCodeResult.Data)) {
            return createCodeResult.Status switch {
                ServiceOperationStatus.NotFound => Success(),
                _ => BadRequest(createCodeResult.ErrorMessage)
            };
        }

        await _emailVerificationService.SendConfirmationEmailAsync(user, createCodeResult.Data);

        return Success();
    }

    public async Task<Response> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken) {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null) {
            return Success();
        }

        var confirmResult = await _emailVerificationService.ConfirmEmailAsync(user.Id, request.Code);

        if (confirmResult.Status == ServiceOperationStatus.Succeeded) {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return confirmResult.Status switch {
            ServiceOperationStatus.Succeeded => Success(message: "Email confirmed successfully."),
            ServiceOperationStatus.NotFound => BadRequest(confirmResult.ErrorMessage ?? "Invalid code."),
            _ => BadRequest(confirmResult.ErrorMessage ?? "Failed to confirm email.")
        };
    }


}
