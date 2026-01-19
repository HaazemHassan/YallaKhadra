using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.WasteReports.Queries.Models;
using YallaKhadra.Core.Features.WasteReports.Queries.Responses;
using YallaKhadra.Core.Helpers;

namespace YallaKhadra.Core.Features.WasteReports.Queries.Handlers {
    public class WasteReportQueryHandler : ResponseHandler,
                                            IRequestHandler<GetWasteReportByIdQuery, Response<WasteReportResponse>>,
                                            IRequestHandler<GetWasteReportsPaginatedQuery, PaginatedResult<WasteReportResponse>>,
                                            IRequestHandler<GetReportsNearLocationQuery, Response<List<WasteReportResponse>>> {

        private readonly IWasteReportRepository _wasteReportRepository;
        private readonly IMapper _mapper;

        public WasteReportQueryHandler(
            IWasteReportRepository wasteReportRepository,
            IMapper mapper) {
            _wasteReportRepository = wasteReportRepository;
            _mapper = mapper;
        }

        public async Task<Response<WasteReportResponse>> Handle(
            GetWasteReportByIdQuery request,
            CancellationToken cancellationToken) {
            try {
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

                return Success(response);
            }
            catch (Exception ex) {
                return BadRequest<WasteReportResponse>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<PaginatedResult<WasteReportResponse>> Handle(
            GetWasteReportsPaginatedQuery request,
            CancellationToken cancellationToken) {
            try {
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
            catch (Exception ex) {
                return PaginatedResult<WasteReportResponse>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response<List<WasteReportResponse>>> Handle(
            GetReportsNearLocationQuery request,
            CancellationToken cancellationToken) {
            try {
                // Get all waste reports with related data
                var allReports = await _wasteReportRepository
                    .GetTableNoTracking()
                    .Include(r => r.Images)
                    .Include(r => r.User)
                    .ToListAsync(cancellationToken);

                // Filter reports within the specified radius using Haversine formula
                var nearbyReports = allReports
                    .Where(r => {
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
            catch (Exception ex) {
                return BadRequest<List<WasteReportResponse>>($"An error occurred: {ex.Message}");
            }
        }
    }
}
