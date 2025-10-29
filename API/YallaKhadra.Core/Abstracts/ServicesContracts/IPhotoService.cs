using Microsoft.AspNetCore.Http;
using YallaKhadra.Core.Bases;

namespace YallaKhadra.Core.Abstracts.ServicesContracts
{
    public interface IPhotoService
    {
        Task<ServiceOperationResult<string>> UploadRportPhotos(IList<IFormFile>? photos, int reportId);
    }
}
