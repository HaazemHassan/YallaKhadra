using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly IImageService<UserProfileImage> _imageService;

        public UserCommandHanlder(IUnitOfWork unitOfWork, IMapper mapper, IApplicationUserService applicationUserService, ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager, IAuthenticationService authenticationService, IImageService<UserProfileImage> imageService) {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _applicationUserService = applicationUserService;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _authenticationService = authenticationService;
            _imageService = imageService;
        }


        public async Task<Response<AuthResult>> Handle(RegisterCommand request, CancellationToken cancellationToken) {
            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            UserProfileImage? uploadedImage = null;
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
                if (!addToRoleResult.Succeeded) {
                    await _unitOfWork.RollbackAsync();
                    return BadRequest<AuthResult>("Failed to assign user to role");
                }

                // Upload profile image if provided
                if (request.ProfileImage != null && request.ProfileImage.Length > 0) {
                    uploadedImage = await _imageService.UploadWithoutSaveAsync(
                        request.ProfileImage,
                        user.Id,
                        user.Id,
                        cancellationToken);

                    user.ProfileImageId = uploadedImage.Id;
                    await _userManager.UpdateAsync(user);

                    // Reload user with ProfileImage
                    user = await _userManager.Users
                        .Include(u => u.ProfileImage)
                        .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);
                }

                var authResult = await _authenticationService.AuthenticateAsync(user!);
                if (authResult.Status != ServiceOperationStatus.Succeeded || authResult.Data is null) {
                    if (uploadedImage != null) {
                        await _imageService.DeleteAsync(uploadedImage);
                    }
                    await _unitOfWork.RollbackAsync();
                    return BadRequest<AuthResult>(authResult.ErrorMessage);
                }

                authResult.Data.User = _mapper.Map<GetUserByIdResponse>(user);
                await _unitOfWork.CommitAsync();
                return Created(authResult.Data);
            }
            catch (Exception ex) {
                if (uploadedImage != null) {
                    await _imageService.DeleteAsync(uploadedImage);
                }
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

                // Reload user with ProfileImage
                var userWithImage = await _userManager.Users
                    .Include(u => u.ProfileImage)
                    .FirstOrDefaultAsync(u => u.Id == addedUser.Id, cancellationToken);

                var response = _mapper.Map<AddUserResponse>(userWithImage);
                await _unitOfWork.CommitAsync();
                return Created(response);
            }
            catch (Exception ex) {
                await _unitOfWork.RollbackAsync();
                return BadRequest<AddUserResponse>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response> Handle(UpdateUserCommand request, CancellationToken cancellationToken) {

            UserProfileImage? newProfileImage = null;

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try {

                var userFromDb = await _userManager.Users
                    .Include(u => u.ProfileImage)
                    .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

                if (userFromDb is null)
                    return NotFound("User not found");


                var oldProfileImage = userFromDb.ProfileImage;

                if (request.ProfileImage is not null) {
                    newProfileImage = await _imageService.UploadAsync(
                        request.ProfileImage,
                        _currentUserService.UserId!.Value,
                        userFromDb.Id,
                        cancellationToken);

                    userFromDb.ProfileImageId = newProfileImage.Id;
                }

                var userMapped = _mapper.Map(request, userFromDb);
                var updateResult = await _userManager.UpdateAsync(userMapped);
                if (!updateResult.Succeeded) {
                    if (newProfileImage is not null)
                        await _imageService.DeleteAsync(newProfileImage);

                    await _unitOfWork.RollbackAsync();
                    return BadRequest("Update failed.");
                }

                if (newProfileImage != null && oldProfileImage != null)
                    await _imageService.DeleteAsync(oldProfileImage);


                await _unitOfWork.CommitAsync();
                return Updated("User updated successfully");
            }
            catch (Exception ex) {
                if (newProfileImage != null)
                    await _imageService.DeleteAsync(newProfileImage);

                await _unitOfWork.RollbackAsync();
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }



    }
}
