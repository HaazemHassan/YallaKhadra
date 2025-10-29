using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Abstracts.ServicesContracts
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadPhotoAsync(IFormFile file, PhotoType type);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
