using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Users.Queries.Models;
using YallaKhadra.Core.Features.Users.Queries.Responses;

namespace YallaKhadra.Core.Features.Users.Queries.Handlers {
    public class UserQueryHandler : ResponseHandler,
                                    IRequestHandler<GetUsersPaginatedQuery, PaginatedResult<GetUsersPaginatedResponse>>,
                                    IRequestHandler<GetUsersByRoleQuery, PaginatedResult<GetUsersByRoleResponse>>,
                                    IRequestHandler<GetUserDetailsQuery, Response<GetUserDetailsResponse>>,
                                    IRequestHandler<GetWorkerDetailsQuery, Response<GetWorkerDetailsResponse>>,

                                    IRequestHandler<GetUserByIdQuery, Response<GetUserByIdResponse>>,
                                    IRequestHandler<CheckEmailAvailabilityQuery, Response<bool>> {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IApplicationUserService _applicationUserService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWasteReportRepository _wasteReportRepository;
        private readonly ICleanupTaskRepository _cleanupTaskRepository;

        public UserQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper, IApplicationUserService applicationUserService, ICurrentUserService currentUserService, IWasteReportRepository wasteReportRepository, ICleanupTaskRepository cleanupTaskRepository) {
            _userManager = userManager;
            _mapper = mapper;
            _applicationUserService = applicationUserService;
            _currentUserService = currentUserService;
            _wasteReportRepository = wasteReportRepository;
            _cleanupTaskRepository = cleanupTaskRepository;
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

        public async Task<PaginatedResult<GetUsersByRoleResponse>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken) {
            try {
                var users = await _userManager.GetUsersInRoleAsync(request.Role.ToString());

                var response = users.Select(u => new GetUsersByRoleResponse {
                    Name = $"{u.FirstName} {u.LastName}".Trim(),
                    Email = u.Email ?? string.Empty,
                    PhoneNumber = u.PhoneNumber ?? string.Empty,
                    Address = u.Address ?? string.Empty,
                    IsLocked = u.LockoutEnd.HasValue && u.LockoutEnd > DateTimeOffset.UtcNow,
                    Points = request.Role == UserRole.User ? u.PointsBalance : null
                }).AsQueryable();

                return await response.ToPaginatedResultAsync(request.PageNumber, request.PageSize);
            }
            catch (Exception ex) {
                return PaginatedResult<GetUsersByRoleResponse>.Failure($"An error occurred: {ex.Message}");
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

        public async Task<Response<GetUserDetailsResponse>> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken) {
            try {
                var user = await _userManager.Users
                    .Include(u => u.ProfileImage)
                    .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

                if (user is null)
                    return NotFound<GetUserDetailsResponse>("User not found");

                var userReports = _wasteReportRepository.GetTableNoTracking(r => r.UserId == request.Id);

                var reportCounts = await userReports
                    .GroupBy(r => 1)
                    .Select(g => new {
                        Pending = g.Count(r => r.Status == ReportStatus.Pending),
                        InProgress = g.Count(r => r.Status == ReportStatus.InProgress),
                        Done = g.Count(r => r.Status == ReportStatus.Done)
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                var response = new GetUserDetailsResponse {
                    Name = $"{user.FirstName} {user.LastName}".Trim(),
                    Email = user.Email ?? string.Empty,
                    ProfileImage = user.ProfileImage is null
                        ? null
                        : new UserProfileImageDto {
                            Id = user.ProfileImage.Id,
                            Url = user.ProfileImage.Url
                        },
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Address = user.Address ?? string.Empty,
                    Points = user.PointsBalance,
                    PendingReportsCount = reportCounts?.Pending ?? 0,
                    InProgressReportsCount = reportCounts?.InProgress ?? 0,
                    DoneReportsCount = reportCounts?.Done ?? 0
                };

                return Success(response);
            }
            catch (Exception ex) {
                return BadRequest<GetUserDetailsResponse>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response<GetWorkerDetailsResponse>> Handle(GetWorkerDetailsQuery request, CancellationToken cancellationToken) {
            try {
                var worker = await _userManager.Users
                    .Include(u => u.ProfileImage)
                    .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

                if (worker is null)
                    return NotFound<GetWorkerDetailsResponse>("Worker not found");

                var isWorker = await _userManager.IsInRoleAsync(worker, UserRole.Worker.ToString());
                if (!isWorker)
                    return BadRequest<GetWorkerDetailsResponse>("The selected user is not a worker.");

                var completedTasks = await _cleanupTaskRepository
                    .GetTableNoTracking(t => t.WorkerId == request.Id && t.CompletedAt.HasValue)
                    .Include(t => t.Report)
                    .ToListAsync(cancellationToken);

                var totalCleanups = completedTasks.Count;
                var avgResponseTimeHours = totalCleanups == 0
                    ? 0
                    : completedTasks.Average(t => (t.AssignedAt - t.Report.CreatedAt).TotalHours);
                var totalHours = completedTasks.Sum(t => (t.CompletedAt!.Value - t.AssignedAt).TotalHours);

                var response = new GetWorkerDetailsResponse {
                    Name = $"{worker.FirstName} {worker.LastName}".Trim(),
                    Email = worker.Email ?? string.Empty,
                    ProfileImage = worker.ProfileImage is null
                        ? null
                        : new UserProfileImageDto {
                            Id = worker.ProfileImage.Id,
                            Url = worker.ProfileImage.Url
                        },
                    PhoneNumber = worker.PhoneNumber ?? string.Empty,
                    Address = worker.Address ?? string.Empty,
                    TotalCleanups = totalCleanups,
                    AvgResponseTime = Math.Round(avgResponseTimeHours, 2),
                    TotalHours = Math.Round(totalHours, 2)
                };

                return Success(response);
            }
            catch (Exception ex) {
                return BadRequest<GetWorkerDetailsResponse>($"An error occurred: {ex.Message}");
            }
        }
    }
}
