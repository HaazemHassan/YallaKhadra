using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Users.Commands.RequestModels;
using YallaKhadra.Core.Features.Users.Commands.Responses;

namespace YallaKhadra.Core.Features.Users.Commands.Handlers {
    public class UserCommandHanlder : ResponseHandler, IRequestHandler<AddUserCommand, Response<AddUserResponse>> {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IApplicationUserService _applicationUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public UserCommandHanlder(IUnitOfWork unitOfWork, IMapper mapper, IApplicationUserService applicationUserService, ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager) {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _applicationUserService = applicationUserService;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<Response<AddUserResponse>> Handle(AddUserCommand request, CancellationToken cancellationToken) {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
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


    }
}
