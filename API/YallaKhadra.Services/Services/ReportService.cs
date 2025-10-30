using Microsoft.AspNetCore.Http;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.Services.Services
{
    public class ReportService : IReportService
    {
        private readonly ICurrentUserService _currentUserServic;
        private IReportRepository _reportRepository;
        public ReportService(ICurrentUserService currentUserService, IReportRepository reportRepository)
        {
            _currentUserServic = currentUserService;
            _reportRepository = reportRepository;
        }

        public async Task<ServiceOperationResult<Report?>> AddReportAsync(Report report, IList<IFormFile>? Photos)
        {
            if (report == null)
                return ServiceOperationResult<Report?>.Failure(ServiceOperationStatus.InvalidParameters, "Report cannot be null");

            var userId = _currentUserServic.UserId;


            if (userId == null)
                return ServiceOperationResult<Report?>.Failure(ServiceOperationStatus.Unauthorized, "User is not authenticated");

            report.UserId = userId.Value;
            var addedReport = await _reportRepository.AddAsync(report);
            if (addedReport == null)
                return ServiceOperationResult<Report?>.Failure(ServiceOperationStatus.Failed, "Failed to add report");



            return ServiceOperationResult<Report?>.Success(addedReport);
        }

        public async Task<ServiceOperationResult<Report?>> ReviewReportAsync(int reportId, bool isApproved, string? notes, Guid reviewedById)
        {
            // Get the report
            var report = await _reportRepository.GetAsync(reportId);
            if (report == null)
                return ServiceOperationResult<Report?>.Failure(ServiceOperationStatus.NotFound, "Report not found");


            if (report.Status != ReportStatus.Pending)
                return ServiceOperationResult<Report?>.Failure(ServiceOperationStatus.InvalidParameters, $"Report is already {report.Status}. Only pending reports can be reviewed.");

            // Update report
            report.Status = isApproved ? ReportStatus.InProgress : ReportStatus.Rejected;
            report.Notes = notes;
            report.ReviewedById = reviewedById;
            report.ReviewedAt = DateTime.UtcNow;

            await _reportRepository.UpdateAsync(report);

            return ServiceOperationResult<Report?>.Success(report);
        }
    }
}
