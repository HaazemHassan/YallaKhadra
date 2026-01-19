using Microsoft.AspNetCore.Http;

namespace YallaKhadra.Core.Abstracts.ServicesContracts
{
    public interface ICloudinaryService
    {
        Task<(string Url, string PublicId)> UploadAsync(
            IFormFile file,
            string folder,
            CancellationToken cancellationToken = default
        );

        Task DeleteAsync(string publicId);
    }
}
