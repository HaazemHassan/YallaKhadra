using Microsoft.AspNetCore.Http;
using YallaKhadra.Core.Entities.BaseEntities;

namespace YallaKhadra.Core.Abstracts.ServicesContracts {
    public interface IImageService<T> where T : BaseImage {
        Task<T> UploadAsync(
            IFormFile file,
            int uploadedBy,
            int ownerId,
            CancellationToken cancellationToken = default
        );


        Task<T> UploadWithoutSaveAsync(
           IFormFile file,
           int uploadedBy,
           int ownerId,
           CancellationToken cancellationToken = default
       );

        Task DeleteAsync(T image);
    }
}
