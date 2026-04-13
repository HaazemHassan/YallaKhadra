using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.CleanupTasks.Queries.Models;
using YallaKhadra.Core.Features.CleanupTasks.Queries.Responses;

namespace YallaKhadra.Core.Features.CleanupTasks.Queries.Handlers {
    public class CleanupTaskQueryHandler : ResponseHandler,
                                            IRequestHandler<GetMyUncompletedTasksQuery, Response<List<CleanupTaskResponse>>>,

                                            IRequestHandler<GetAllCleanupTasksQuery, PaginatedResult<CleanupTaskResponse>>,

                                            IRequestHandler<GetMyWorkOverviewQuery, Response<CleanupWorkOverviewResponse>>,

                                            IRequestHandler<GetMyMainOverviewQuery, Response<CleanupMainOverviewResponse>> {

        private readonly ICleanupTaskRepository _cleanupTaskRepository;
        private readonly ICleanupTaskService _cleanupTaskService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CleanupTaskQueryHandler(
            ICleanupTaskRepository cleanupTaskRepository,
            ICleanupTaskService cleanupTaskService,
            ICurrentUserService currentUserService,
            IMapper mapper) {
            _cleanupTaskRepository = cleanupTaskRepository;
            _cleanupTaskService = cleanupTaskService;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Response<List<CleanupTaskResponse>>> Handle(GetMyUncompletedTasksQuery request,CancellationToken cancellationToken)
        {
            try {
                // Get current user ID
                var userId = _currentUserService.UserId;
                if (!userId.HasValue)
                    return Unauthorized<List<CleanupTaskResponse>>("User is not authenticated.");

                if (!_currentUserService.IsInRole(UserRole.Worker))
                    return Unauthorized<List<CleanupTaskResponse>>("Only workers can view cleanup tasks.");

                // Get worker's uncompleted tasks
                var tasks = await _cleanupTaskService.GetWorkerUncompletedTasksAsync(userId.Value, cancellationToken);

                // Maping
                var response = _mapper.Map<List<CleanupTaskResponse>>(tasks);

                return Success(response, message: $"Retrieved {response.Count} uncompleted tasks.");
            }
            catch (Exception ex) {
                return BadRequest<List<CleanupTaskResponse>>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<PaginatedResult<CleanupTaskResponse>> Handle(GetAllCleanupTasksQuery request,CancellationToken cancellationToken)
        {
            try {
                // Get all cleanup tasks with related data, sorted by date (newest first)
                var cleanupTasksQuery = _cleanupTaskRepository
                    .GetTableNoTracking()
                    .Include(t => t.Report)
                        .ThenInclude(r => r.Images)
                    .Include(t => t.Report)
                        .ThenInclude(r => r.User)
                    .Include(t => t.Worker)
                    .Include(t => t.Images)
                    .OrderByDescending(t => t.AssignedAt);

                // Project to DTO and paginate
                var paginatedResult = await cleanupTasksQuery
                    .ProjectTo<CleanupTaskResponse>(_mapper.ConfigurationProvider)
                    .ToPaginatedResultAsync(request.PageNumber, request.PageSize);

                return paginatedResult;
            }
            catch (Exception ex) {
                return PaginatedResult<CleanupTaskResponse>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response<CleanupWorkOverviewResponse>> Handle(GetMyWorkOverviewQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUserService.UserId;
                if (!userId.HasValue)
                    return Unauthorized<CleanupWorkOverviewResponse>("User is not authenticated.");

                if (!_currentUserService.IsInRole(UserRole.Worker))
                    return Unauthorized<CleanupWorkOverviewResponse>("Only workers can view work overview.");

                var completedTasks = await _cleanupTaskRepository
                    .GetTableNoTracking(t => t.WorkerId == userId.Value && t.CompletedAt.HasValue)
                    .ToListAsync(cancellationToken);

                var response = new CleanupWorkOverviewResponse
                {
                    CompletedCleanupsCount = completedTasks.Count,
                    TotalHours = completedTasks.Sum(t => (t.CompletedAt!.Value - t.AssignedAt).TotalHours),
                    TotalWeightInKg = completedTasks.Sum(t => t.FinalWeightInKg ?? 0m)
                };

                return Success(response, message: "Work overview retrieved successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest<CleanupWorkOverviewResponse>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response<CleanupMainOverviewResponse>> Handle(GetMyMainOverviewQuery request, CancellationToken cancellationToken) {
            try {
                var userId = _currentUserService.UserId;
                if (!userId.HasValue)
                    return Unauthorized<CleanupMainOverviewResponse>("User is not authenticated.");

                if (!_currentUserService.IsInRole(UserRole.Worker))
                    return Unauthorized<CleanupMainOverviewResponse>("Only workers can view main overview.");

                var completedTasks = await _cleanupTaskRepository
                    .GetTableNoTracking(t => t.WorkerId == userId.Value && t.CompletedAt.HasValue)
                    .ToListAsync(cancellationToken);

                var response = new CleanupMainOverviewResponse {
                    CompletedCleanupsCount = completedTasks.Count,
                    AverageHours = completedTasks.Count == 0
                        ? 0
                        : completedTasks.Average(t => (t.CompletedAt!.Value - t.AssignedAt).TotalHours)
                };

                return Success(response, message: "Main overview retrieved successfully.");
            }
            catch (Exception ex) {
                return BadRequest<CleanupMainOverviewResponse>($"An error occurred: {ex.Message}");
            }
        }
    }
}
