using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities.GreenEntities;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.Services.Services
{
    public class AIWasteScanService : IAIWasteScanService
    {
        private readonly string _aiApiUrl;

        private readonly IAIWasteScanRepository _scanRepository;
        private readonly IImageService<WasteScanImage> _imageService;
        private readonly IHttpClientFactory _httpClientFactory;

        public AIWasteScanService(
            IAIWasteScanRepository scanRepository,
            IImageService<WasteScanImage> imageService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _scanRepository = scanRepository;
            _imageService = imageService;
            _httpClientFactory = httpClientFactory;
            _aiApiUrl = configuration["AISettings:ApiUrl"]
                ?? throw new InvalidOperationException("AISettings:ApiUrl is not configured.");
        }

        public async Task<ServiceOperationResult<AIWasteScan>> CreateScanAsync(IFormFile image, int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (image == null || image.Length == 0)
                    return ServiceOperationResult<AIWasteScan>.Failure(ServiceOperationStatus.Failed, "Image is required.")!;

                var aiResult = await CallAiPredictionApiAsync(image, cancellationToken);

                if (aiResult == null)
                    return ServiceOperationResult<AIWasteScan>.Failure(ServiceOperationStatus.Failed, "AI prediction service is currently unavailable. Please try again later.")!;

                var scan = new AIWasteScan
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    AIPredictedType = aiResult.Prediction,
                    AIIsRecyclable = aiResult.AIIsRecyclable,
                    AIExplanation = aiResult.AIExplanation
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

        private async Task<AiPredictionResponse?> CallAiPredictionApiAsync(IFormFile image, CancellationToken cancellationToken)
        {
            using var httpClient = _httpClientFactory.CreateClient();

            using var content = new MultipartFormDataContent();
            using var stream = image.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType ?? "image/jpeg");
            content.Add(fileContent, "file", image.FileName);

            var response = await httpClient.PostAsync(_aiApiUrl, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<AiPredictionResponse>(cancellationToken: cancellationToken);
        }

        private sealed class AiPredictionResponse
        {
            [JsonPropertyName("prediction")]
            public string Prediction { get; set; } = string.Empty;

            [JsonPropertyName("confidence")]
            public double Confidence { get; set; }

            [JsonPropertyName("AIIsRecyclable")]
            public bool AIIsRecyclable { get; set; }

            [JsonPropertyName("AIExplanation")]
            public string AIExplanation { get; set; } = string.Empty;
        }
    }
}

