using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.Services.Services {
    public class WasteReportService : IWasteReportService {
        private readonly IWasteReportRepository _wasteReportRepository;
        private readonly IImageService<ReportImage> _imageService;

        public WasteReportService(
            IWasteReportRepository wasteReportRepository,
            IImageService<ReportImage> imageService) {
            _wasteReportRepository = wasteReportRepository;
            _imageService = imageService;
        }

        public async Task<ServiceOperationResult<WasteReport>> CreateAsync(
            WasteReport wasteReport,
            List<IFormFile>? images,
            CancellationToken cancellationToken = default) {
            try {
                // Add waste report to database
               var AddedWasteReport = await _wasteReportRepository.AddAsync(wasteReport);

                // Upload images if provided
                if (images != null && images.Count > 0) {
                    foreach (var image in images) {
                        if (image.Length > 0) {
                            await _imageService.UploadAsync(
                                image,
                                AddedWasteReport.UserId,
                                AddedWasteReport.Id,
                                cancellationToken);
                        }
                    }
                }

                return ServiceOperationResult<WasteReport>.Success(wasteReport)!;
            }
            catch (Exception ex) {
                return ServiceOperationResult<WasteReport>.Failure(
                    ServiceOperationStatus.Failed,
                    $"Failed to create waste report: {ex.Message}")!;
            }
        }

        public async Task<WasteReport?> GetByIdAsync(int id, CancellationToken cancellationToken = default) {
            return await _wasteReportRepository
                .GetTableNoTracking(r => r.Id == id)
                .Include(r => r.Images)
                .Include(r => r.User)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
