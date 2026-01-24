using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Authentication;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Users.Commands.RequestModels;
using YallaKhadra.Core.Features.Users.Commands.Responses;
using YallaKhadra.Core.Features.Users.Queries.Responses;

namespace YallaKhadra.Core.Features.Users.Commands.Handlers {
    public class UserCommandHanlder : ResponseHandler, IRequestHandler<RegisterCommand, Response<AuthResult>>,
                                                       IRequestHandler<AddUserCommand, Response<AddUserResponse>>,
                                                       IRequestHandler<UpdateUserCommand, Response> {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IApplicationUserService _applicationUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthenticationService _authenticationService;

        public UserCommandHanlder(IUnitOfWork unitOfWork, IMapper mapper, IApplicationUserService applicationUserService, ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager, IAuthenticationService authenticationService) {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _applicationUserService = applicationUserService;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _authenticationService = authenticationService;
        }


        public async Task<Response<AuthResult>> Handle(RegisterCommand request, CancellationToken cancellationToken) {
            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try {
                var userMapped = _mapper.Map<ApplicationUser>(request);
                var addUserResult = await _applicationUserService.AddUser(userMapped, request.Password);

                if (addUserResult.Status != ServiceOperationStatus.Succeeded || addUserResult.Data is null) {
                    await _unitOfWork.RollbackAsync();
                    return addUserResult.Status switch {
                        ServiceOperationStatus.AlreadyExists => Conflict<AuthResult>(addUserResult.ErrorMessage),
                        _ => BadRequest<AuthResult>(addUserResult.ErrorMessage),
                    };
                }


                var user = addUserResult.Data;

                var addToRoleResult = await _userManager.AddToRoleAsync(user, UserRole.User.ToString());
                if (!addToRoleResult.Succeeded)
                    return BadRequest<AuthResult>("Failed to assign user to role");

                var authResult = await _authenticationService.AuthenticateAsync(user);
                if (authResult.Status != ServiceOperationStatus.Succeeded || authResult.Data is null) {
                    await _unitOfWork.RollbackAsync();
                    return BadRequest<AuthResult>(authResult.ErrorMessage);
                }

                authResult.Data.User = _mapper.Map<GetUserByIdResponse>(user);
                await _unitOfWork.CommitAsync();
                return Created(authResult.Data);
            }
            catch (Exception ex) {
                await _unitOfWork.RollbackAsync();
                return BadRequest<AuthResult>($"An error occurred: {ex.Message}");
            }
        }


        public async Task<Response<AddUserResponse>> Handle(AddUserCommand request, CancellationToken cancellationToken) {
            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try {
                var userMapped = _mapper.Map<ApplicationUser>(request);
                var addUserResult = await _applicationUserService.AddUser(userMapped, request.Password);

                if (addUserResult.Status != ServiceOperationStatus.Succeeded || addUserResult.Data is null) {
                    await _unitOfWork.RollbackAsync();
                    return addUserResult.Status switch {
                        ServiceOperationStatus.AlreadyExists => Conflict<AddUserResponse>(addUserResult.ErrorMessage),
                        _ => BadRequest<AddUserResponse>(addUserResult.ErrorMessage),
                    };
                }



                var role = request.UserRole;
                if (_currentUserService.IsInRole(UserRole.Admin)) {
                    if (role == UserRole.SuperAdmin ||
                         role == UserRole.Admin) {
                        await _unitOfWork.RollbackAsync();
                        return Forbid<AddUserResponse>("You dont' meet the permissions");
                    }
                }


                var addedUser = addUserResult.Data;
                var addToRoleResult = await _userManager.AddToRoleAsync(addedUser, role.ToString()!);
                if (!addToRoleResult.Succeeded) {
                    await _unitOfWork.RollbackAsync();
                    return BadRequest<AddUserResponse>("Failed to assign user to role");
                }


                var response = _mapper.Map<AddUserResponse>(addedUser);
                await _unitOfWork.CommitAsync();
                return Created(response);
            }
            catch (Exception ex) {
                await _unitOfWork.RollbackAsync();
                return BadRequest<AddUserResponse>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response> Handle(UpdateUserCommand request, CancellationToken cancellationToken) {
            ApplicationUser? userFromDb = await _userManager.FindByIdAsync(request.Id.ToString());
            if (userFromDb is null)
                return NotFound("User not found");


            var userMapped = _mapper.Map(request, userFromDb);
            var updateResult = await _userManager.UpdateAsync(userMapped);
            if (updateResult.Succeeded)
                return Updated("User updated successfully");

            return BadRequest("Update failed.");


        }



    }
}
