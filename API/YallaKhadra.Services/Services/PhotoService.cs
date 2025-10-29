using Microsoft.AspNetCore.Http;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.Services.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IPhotoRepository _photoRepository;
        public PhotoService(ICloudinaryService cloudinaryService, IPhotoRepository photoRepository)
        {
            _cloudinaryService = cloudinaryService;
            _photoRepository = photoRepository;
        }
        public async Task<ServiceOperationResult<string>> UploadRportPhotos(IList<IFormFile>? photos, int reportId)
        {
            if (photos == null || photos.Count <= 0)
                return ServiceOperationResult<string>.Failure(ServiceOperationStatus.InvalidParameters, "No photos to upload");

            List<Photo> NewPhotos = new List<Photo>();

            foreach (var photoFile in photos)
            {
                if (photoFile != null)
                {
                    var addPhotoResult = await _cloudinaryService.UploadPhotoAsync(photoFile, PhotoType.Report);
                    var photo = new Photo
                    {
                        Url = addPhotoResult.Url.ToString(),
                        PublicId = addPhotoResult.PublicId,
                        ReportId = reportId,
                        Type = PhotoType.Report
                    };
                    NewPhotos.Add(photo);
                }
            }

            var result = _photoRepository.AddRangeAsync(NewPhotos);
            return ServiceOperationResult<string>.Success("Photos uploaded successfully"!);
        }
    }
}
