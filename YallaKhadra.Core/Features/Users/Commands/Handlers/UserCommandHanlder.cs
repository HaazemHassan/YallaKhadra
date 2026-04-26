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

namespace YallaKhadra.Core.Features.Users.Commands.Handlers {
    public class UserCommandHanlder : ResponseHandler, IRequestHandler<RegisterCommand, Response>,
                                                       IRequestHandler<AddUserCommand, Response<AddUserResponse>>,
                                                       IRequestHandler<UpdateUserCommand, Response>,
                                                       IRequestHandler<ToggleUserLockCommand, Response<ToggleUserLockResponse>> {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IApplicationUserService _applicationUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IEmailVerificationService _emailVerificationService;
        private readonly IImageService<UserProfileImage> _imageService;

        public UserCommandHanlder(IUnitOfWork unitOfWork, IMapper mapper, IApplicationUserService applicationUserService, ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager, IAuthenticationService authenticationService, IEmailVerificationService emailVerificationService, IImageService<UserProfileImage> imageService) {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _applicationUserService = applicationUserService;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _authenticationService = authenticationService;
            _emailVerificationService = emailVerificationService;
            _imageService = imageService;
        }


        public async Task<Response> Handle(RegisterCommand request, CancellationToken cancellationToken) {
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

                if (request.ProfileImage != null && request.ProfileImage.Length > 0) {
                    uploadedImage = await _imageService.UploadWithoutSaveAsync(
                        request.ProfileImage,
                        user.Id,
                        user.Id,
                        cancellationToken);

                    user.ProfileImageId = uploadedImage.Id;
                    await _userManager.UpdateAsync(user);

                    user = await _userManager.Users
                        .Include(u => u.ProfileImage)
                        .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);
                }

                if (user is null) {
                    await _unitOfWork.RollbackAsync();
                    return BadRequest<AuthResult>("User not found.");
                }

                var createCodeResult = await _emailVerificationService.CreateEmailConfirmationCodeAsync(user.Id);
                if (createCodeResult.Status != ServiceOperationStatus.Succeeded || string.IsNullOrWhiteSpace(createCodeResult.Data)) {
                    if (uploadedImage != null) {
                        await _imageService.DeleteAsync(uploadedImage);
                    }

                    await _unitOfWork.RollbackAsync();
                    return BadRequest<AuthResult>(createCodeResult.ErrorMessage ?? "Failed to create email confirmation code.");
                }

                await _emailVerificationService.SendConfirmationEmailAsync(user, createCodeResult.Data);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return Created();
            }
            catch (Exception ex) {
                if (uploadedImage != null) {
                    await _imageService.DeleteAsync(uploadedImage);
                }
                await _unitOfWork.RollbackAsync();
                throw;
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

                var createCodeResult = await _emailVerificationService.CreateEmailConfirmationCodeAsync(addedUser.Id);
                if (createCodeResult.Status != ServiceOperationStatus.Succeeded || string.IsNullOrWhiteSpace(createCodeResult.Data)) {
                    await _unitOfWork.RollbackAsync();
                    return BadRequest<AddUserResponse>(createCodeResult.ErrorMessage ?? "Failed to create email confirmation code.");
                }

                await _emailVerificationService.SendConfirmationEmailAsync(addedUser, createCodeResult.Data);

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

        public async Task<Response<ToggleUserLockResponse>> Handle(ToggleUserLockCommand request, CancellationToken cancellationToken) {
            try {
                var user = await _userManager.FindByIdAsync(request.Id.ToString());
                if (user is null)
                    return NotFound<ToggleUserLockResponse>("User not found");

                var isCurrentlyLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow;

                if (isCurrentlyLocked) {
                    var unlockResult = await _userManager.SetLockoutEndDateAsync(user, null);
                    if (!unlockResult.Succeeded)
                        return BadRequest<ToggleUserLockResponse>("Failed to unlock user.");
                }
                else {
                    if (!user.LockoutEnabled) {
                        var enableLockoutResult = await _userManager.SetLockoutEnabledAsync(user, true);
                        if (!enableLockoutResult.Succeeded)
                            return BadRequest<ToggleUserLockResponse>("Failed to enable lockout for user.");
                    }

                    var lockResult = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
                    if (!lockResult.Succeeded)
                        return BadRequest<ToggleUserLockResponse>("Failed to lock user.");
                }

                return Success(new ToggleUserLockResponse {
                    UserId = user.Id,
                    IsLocked = !isCurrentlyLocked
                }, message: isCurrentlyLocked ? "User unlocked successfully." : "User locked successfully.");
            }
            catch (Exception ex) {
                return BadRequest<ToggleUserLockResponse>($"An error occurred: {ex.Message}");
            }
        }



    }
}
