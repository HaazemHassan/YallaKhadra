using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Entities;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Reports.Queries.Models;
using YallaKhadra.Core.Features.Reports.Queries.Responses;

namespace YallaKhadra.Core.Features.Reports.Queries.Handlers
{
    public class ReportQueryHandler : ResponseHandler,
                                      IRequestHandler<GetPendingReportsPaginatedQuery, PaginatedResult<GetPendingReportsPaginatedResponse>>,
                                      IRequestHandler<GetMyReportsPaginatedQuery, PaginatedResult<GetMyReportsPaginatedResponse>>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public ReportQueryHandler(IReportRepository reportRepository, IMapper mapper, ICurrentUserService currentUserService)
        {
            _reportRepository = reportRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedResult<GetPendingReportsPaginatedResponse>> Handle(GetPendingReportsPaginatedQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var reportsQuery = _reportRepository.GetTableNoTracking(r => r.Status == ReportStatus.Pending)
                                                    .Include(r => r.User)
                                                    .Include(r => r.Photos);

                var reportsPaginatedResult = await reportsQuery
                                    .ProjectTo<GetPendingReportsPaginatedResponse>(_mapper.ConfigurationProvider)
                                    .ToPaginatedResultAsync(request.PageNumber, request.PageSize);

                return reportsPaginatedResult;
            }
            catch (Exception ex)
            {
                return PaginatedResult<GetPendingReportsPaginatedResponse>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<PaginatedResult<GetMyReportsPaginatedResponse>> Handle(GetMyReportsPaginatedQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUserService.UserId;
                if (userId == null)
                    return PaginatedResult<GetMyReportsPaginatedResponse>.Failure("User is not authenticated");

                IQueryable<Report> reportsQuery = _reportRepository.GetTableNoTracking(r => r.UserId == userId.Value)
                                                    .Include(r => r.Photos)
                                                    .Include(r => r.ReviewedBy);

                if (request.Status.HasValue)
                {
                    reportsQuery = reportsQuery.Where(r => r.Status == request.Status.Value);
                }

                // Order by date
                reportsQuery = reportsQuery.OrderByDescending(r => r.CreatedAt);

                var reportsPaginatedResult = await reportsQuery
                                    .ProjectTo<GetMyReportsPaginatedResponse>(_mapper.ConfigurationProvider)
                                    .ToPaginatedResultAsync(request.PageNumber, request.PageSize);

                return reportsPaginatedResult;
            }
            catch (Exception ex)
            {
                return PaginatedResult<GetMyReportsPaginatedResponse>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
