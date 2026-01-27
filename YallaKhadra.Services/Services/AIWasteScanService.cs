using Microsoft.AspNetCore.Http;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities.GreenEntities;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.Services.Services
{
    public class AIWasteScanService : IAIWasteScanService
    {
        private readonly IAIWasteScanRepository _scanRepository;
        private readonly IImageService<WasteScanImage> _imageService;

        public AIWasteScanService(IAIWasteScanRepository scanRepository, IImageService<WasteScanImage> imageService)
        {
            _scanRepository = scanRepository;
            _imageService = imageService;
        }

        public async Task<ServiceOperationResult<AIWasteScan>> CreateScanAsync(IFormFile image, int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (image == null || image.Length == 0)
                    return ServiceOperationResult<AIWasteScan>.Failure(ServiceOperationStatus.Failed, "Image is required.")!;

                // TODO: Implement AI Scanner API Integration
                // ============================================
                // Replace the mock data above with actual AI Scanner API call:
                //
                // scan.AIPredictedType = aiResult.WasteType;
                // scan.AIIsRecyclable = aiResult.IsRecyclable;
                // scan.AIExplanation = aiResult.Explanation;
                // ============================================


                var scan = new AIWasteScan
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    AIPredictedType = "Plastic",
                    AIIsRecyclable = true,
                    AIExplanation = "This appears to be recyclable plastic waste. Please dispose of it in the plastic recycling bin." // TODO: Replace with actual AI explanation
                };


                await _scanRepository.AddAsync(scan);

                await _imageService.UploadAsync(image, userId, scan.Id, cancellationToken);

                return ServiceOperationResult<AIWasteScan>.Success(scan)!;
            }
            catch (Exception ex)
            {
                return ServiceOperationResult<AIWasteScan>.Failure(ServiceOperationStatus.Failed, $"Failed to create scan: {ex.Message}")!;
            }
        }
    }
}
