using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.CleanupTasks.Queries.Responses;
using YallaKhadra.Core.Features.WasteReports.Queries.Models;
using YallaKhadra.Core.Features.WasteReports.Queries.Responses;
using YallaKhadra.Core.Helpers;

namespace YallaKhadra.Core.Features.WasteReports.Queries.Handlers
{
    public class WasteReportQueryHandler : ResponseHandler,
                                            IRequestHandler<GetWasteReportByIdQuery, Response<WasteReportResponse>>,
                                            IRequestHandler<GetWasteReportsPaginatedQuery, PaginatedResult<WasteReportResponse>>,
                                            IRequestHandler<GetReportsNearLocationQuery, Response<List<WasteReportResponse>>>,
                                            IRequestHandler<GetPendingReportsQuery, PaginatedResult<WasteReportResponse>>,
                                            IRequestHandler<GetMyReportsQuery, PaginatedResult<WasteReportBriefDto>>
    {

        private readonly IWasteReportRepository _wasteReportRepository;
        private readonly ICleanupTaskRepository _cleanupTaskRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public WasteReportQueryHandler(
            IWasteReportRepository wasteReportRepository,
            ICleanupTaskRepository cleanupTaskRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _wasteReportRepository = wasteReportRepository;
            _cleanupTaskRepository = cleanupTaskRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Response<WasteReportResponse>> Handle(GetWasteReportByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get waste report by ID with related data
                var wasteReport = await _wasteReportRepository
                    .GetTableNoTracking(r => r.Id == request.Id)
                    .Include(r => r.Images)
                    .Include(r => r.User)
                    .FirstOrDefaultAsync(cancellationToken);

                if (wasteReport is null)
                    return NotFound<WasteReportResponse>($"Waste report with ID {request.Id} not found.");

                // Map to response DTO
                var response = _mapper.Map<WasteReportResponse>(wasteReport);

                // If report is Done, fetch cleanup task details
                if (wasteReport.Status == ReportStatus.Done)
                {
                    var cleanupTask = await _cleanupTaskRepository
                        .GetTableNoTracking(ct => ct.ReportId == request.Id)
                        .Include(ct => ct.Images)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (cleanupTask != null)
                    {
                        response.FinalWasteType = cleanupTask.FinalWasteType;
                        response.FinalWasteTypeName = cleanupTask.FinalWasteType?.ToString();
                        response.FinalWeightInKg = cleanupTask.FinalWeightInKg;
                        response.CleanupImages = cleanupTask.Images.Select(img => new CleanupImageDto
                        {
                            Id = img.Id,
                            Url = img.Url,
                            UploadedAt = img.UploadedAt
                        }).ToList();
                    }
                }

                return Success(response);
            }
            catch (Exception ex)
            {
                return BadRequest<WasteReportResponse>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<PaginatedResult<WasteReportResponse>> Handle(GetWasteReportsPaginatedQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get all waste reports with related data, sorted by date descending
                var wasteReportsQuery = _wasteReportRepository
                    .GetTableNoTracking()
                    .Include(r => r.Images)
                    .Include(r => r.User)
                    .OrderByDescending(r => r.CreatedAt);

                // Project to DTO and paginate
                var paginatedResult = await wasteReportsQuery
                    .ProjectTo<WasteReportResponse>(_mapper.ConfigurationProvider)
                    .ToPaginatedResultAsync(request.PageNumber, request.PageSize);

                return paginatedResult;
            }
            catch (Exception ex)
            {
                return PaginatedResult<WasteReportResponse>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response<List<WasteReportResponse>>> Handle(GetReportsNearLocationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get all waste reports with related data
                var allReports = await _wasteReportRepository
                    .GetTableNoTracking(r => r.Status == ReportStatus.Pending)
                    .Include(r => r.Images)
                    .Include(r => r.User)
                    .ToListAsync(cancellationToken);

                // Filter reports within the specified radius using Haversine formula
                var nearbyReports = allReports
                    .Where(r =>
                    {
                        var distance = GeoLocationHelper.CalculateDistance(
                            request.Latitude,
                            request.Longitude,
                            r.Latitude,
                            r.Longitude);
                        return distance <= request.RadiusInKm;
                    })
                    .OrderBy(r => GeoLocationHelper.CalculateDistance(
                        request.Latitude,
                        request.Longitude,
                        r.Latitude,
                        r.Longitude)) // Sort by distance (nearest first)
                    .ToList();

                // Map to response DTOs
                var response = _mapper.Map<List<WasteReportResponse>>(nearbyReports);

                return Success(response, message: $"Found {response.Count} reports within {request.RadiusInKm} km.");
            }
            catch (Exception ex)
            {
                return BadRequest<List<WasteReportResponse>>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<PaginatedResult<WasteReportResponse>> Handle(GetPendingReportsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var pendingReportsQuery = _wasteReportRepository
                    .GetTableNoTracking(r => r.Status == ReportStatus.Pending)
                    .Include(r => r.Images)
                    .Include(r => r.User)
                    .OrderBy(r => r.CreatedAt);


                var paginatedResult = await pendingReportsQuery
                    .ProjectTo<WasteReportResponse>(_mapper.ConfigurationProvider)
                    .ToPaginatedResultAsync(request.PageNumber, request.PageSize);

                return paginatedResult;
            }
            catch (Exception ex)
            {
                return PaginatedResult<WasteReportResponse>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<PaginatedResult<WasteReportBriefDto>> Handle(GetMyReportsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get current user ID
                var userId = _currentUserService.UserId;
                if (!userId.HasValue)
                    return PaginatedResult<WasteReportBriefDto>.Failure("User is not authenticated.");

                // Build query with user filter
                var query = _wasteReportRepository
                    .GetTableNoTracking(r => r.UserId == userId.Value);

                // Apply status filter if provided
                if (request.Status.HasValue)
                {
                    query = query.Where(r => r.Status == request.Status.Value);
                }

                query = query
                    .Include(r => r.Images)
                    .Include(r => r.User)
                    .OrderByDescending(r => r.CreatedAt);

                var paginatedResult = await query
                    .ProjectTo<WasteReportBriefDto>(_mapper.ConfigurationProvider)
                    .ToPaginatedResultAsync(request.PageNumber, request.PageSize);

                return paginatedResult;
            }
            catch (Exception ex)
            {
                return PaginatedResult<WasteReportBriefDto>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
