using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Features.Users.Queries.Models;
using YallaKhadra.Core.Features.Users.Queries.Responses;

namespace YallaKhadra.Core.Features.Users.Queries.Handlers {
    public class UserQueryHandler : ResponseHandler,
                                    IRequestHandler<GetUsersPaginatedQuery, PaginatedResult<GetUsersPaginatedResponse>>,
                                    IRequestHandler<GetUserByIdQuery, Response<GetUserByIdResponse>>,
                                    IRequestHandler<GetUserByUsernameQuery, Response<GetUserByUsernameResponse>>,
                                    IRequestHandler<SearchUsersQuery, Response<List<SearchUsersResponse>>>,
                                    IRequestHandler<CheckUsernameAvailabilityQuery, Response<bool>>,
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
                var usersQuerable = _userManager.Users;
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
                var user = await _userManager.FindByIdAsync(request.Id.ToString());
                if (user is null)
                    return NotFound<GetUserByIdResponse>();

                var userResponse = _mapper.Map<GetUserByIdResponse>(user);
                return Success(userResponse);
            }
            catch (Exception ex) {
                return BadRequest<GetUserByIdResponse>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response<GetUserByUsernameResponse>> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken) {
            try {
                var user = await _userManager.FindByNameAsync(request.Username);
                if (user is null)
                    return NotFound<GetUserByUsernameResponse>("User not found");

                var userResponse = _mapper.Map<GetUserByUsernameResponse>(user);
                return Success(userResponse);
            }
            catch (Exception ex) {
                return BadRequest<GetUserByUsernameResponse>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response<List<SearchUsersResponse>>> Handle(SearchUsersQuery request, CancellationToken cancellationToken) {
            try {
                var query = _userManager.Users.AsQueryable();
                query = query.Where(u => u.UserName == request.Username && u.Id == _currentUserService.UserId);
                var users = await query.ProjectTo<SearchUsersResponse>(_mapper.ConfigurationProvider).ToListAsync();

                if (!users.Any())
                    return Success(new List<SearchUsersResponse>());

                return Success(users);
            }
            catch (Exception ex) {
                return BadRequest<List<SearchUsersResponse>>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response<bool>> Handle(CheckUsernameAvailabilityQuery request, CancellationToken cancellationToken) {
            try {
                var user = await _userManager.FindByNameAsync(request.Username);
                bool isAvailable = user is null;
                return Success(isAvailable);
            }
            catch (Exception ex) {
                return BadRequest<bool>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response<bool>> Handle(CheckEmailAvailabilityQuery request, CancellationToken cancellationToken) {
            try {
                var user = await _userManager.FindByEmailAsync(request.Email);
                bool isAvailable = user is null;
                return Success(isAvailable);
            }
            catch (Exception ex) {
                return BadRequest<bool>($"An error occurred: {ex.Message}");
            }
        }
    }
}
