using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Features.Users.Queries.Models;
using YallaKhadra.Core.Features.Users.Queries.Responses;

namespace YallaKhadra.Core.Features.Users.Queries.Handlers {
    public class UserQueryHandler : ResponseHandler,
                                    IRequestHandler<GetUsersPaginatedQuery, PaginatedResult<GetUsersPaginatedResponse>>,
                                    IRequestHandler<GetUserByIdQuery, Response<GetUserByIdResponse>>,
                                    IRequestHandler<CheckEmailAvailabilityQuery, Response<bool>> {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IApplicationUserService _applicationUserService;
        private readonly ICurrentUserService _currentUserService;

        public UserQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper, IApplicationUserService applicationUserService, ICurrentUserService currentUserService) {
            _userManager = userManager;
            _mapper = mapper;
            _applicationUserService = applicationUserService;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedResult<GetUsersPaginatedResponse>> Handle(GetUsersPaginatedQuery request, CancellationToken cancellationToken) {
            try {
                var usersQuerable = _userManager.Users.Include(u => u.ProfileImage);
                var usersPaginatedResult = await usersQuerable.ProjectTo<GetUsersPaginatedResponse>(_mapper.ConfigurationProvider)
                                    .ToPaginatedResultAsync(request.PageNumber, request.PageSize);
                return usersPaginatedResult;
            }
            catch (Exception ex) {
                return PaginatedResult<GetUsersPaginatedResponse>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response<GetUserByIdResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken) {
            try {
                var user = await _userManager.Users
                    .Include(u => u.ProfileImage)
                    .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
                    
                if (user is null)
                    return NotFound<GetUserByIdResponse>();

                var userResponse = _mapper.Map<GetUserByIdResponse>(user);
                return Success(userResponse);
            }
            catch (Exception ex) {
                return BadRequest<GetUserByIdResponse>($"An error occurred: {ex.Message}");
            }
        }


        public async Task<Response<bool>> Handle(CheckEmailAvailabilityQuery request, CancellationToken cancellationToken) {
            try {
                var user = await _userManager.FindByEmailAsync(request.Email);
                bool isAvailable = user is null;
                return Success(isAvailable, message: isAvailable ? "Email is available." : "Email is not available.");
            }
            catch (Exception ex) {
                return BadRequest<bool>($"An error occurred: {ex.Message}");
            }
        }
    }
}
