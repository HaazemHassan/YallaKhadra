using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Entities.BaseEntities;
using YallaKhadra.Core.Entities.E_CommerceEntities;
using YallaKhadra.Core.Entities.GreenEntities;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Infrastructure.Data;

namespace YallaKhadra.Services.Services {
    public class ImageService<T> : IImageService<T>
    where T : BaseImage, new() {
        private readonly ICloudinaryService _cloudinary;
        private readonly AppDbContext _context;

        public ImageService(
            ICloudinaryService cloudinary,
            AppDbContext context) {
            _cloudinary = cloudinary;
            _context = context;
        }

        public async Task<T> UploadAsync(
            IFormFile file,
            int uploadedBy,
            int ownerId,
            CancellationToken cancellationToken = default) {
            var folder = GetFolderName();

            var uploadResult = await _cloudinary
                .UploadAsync(file, folder, cancellationToken);

            var image = new T {
                Url = uploadResult.Url,
                PublicId = uploadResult.PublicId,
                UploadedBy = uploadedBy,
                UploadedAt = DateTime.UtcNow
            };

            SetOwner(image, ownerId);


            _context.Set<T>().Add(image);
            await _context.SaveChangesAsync(cancellationToken);

            return image;
        }

        public async Task<T> UploadWithoutSaveAsync(
            IFormFile file,
            int uploadedBy,
            int ownerId,
            CancellationToken cancellationToken = default) {
            var folder = GetFolderName();

            var uploadResult = await _cloudinary
                .UploadAsync(file, folder, cancellationToken);

            var image = new T {
                Url = uploadResult.Url,
                PublicId = uploadResult.PublicId,
                UploadedBy = uploadedBy,
                UploadedAt = DateTime.UtcNow
            };

            SetOwner(image, ownerId);

            _context.Set<T>().Add(image);

            return image;
        }

        public async Task DeleteAsync(T image) {
            if (image is UserProfileImage userProfileImage) {
                var user = await _context.Set<ApplicationUser>()
                    .FirstOrDefaultAsync(u => u.Id == userProfileImage.UserId);

                if (user is not null && user.ProfileImageId == image.Id) {
                    user.ProfileImageId = null;
                    user.ProfileImage = null;
                    await _context.SaveChangesAsync();
                }
            }

            await _cloudinary.DeleteAsync(image.PublicId);
            _context.Set<T>().Remove(image);
            await _context.SaveChangesAsync();
        }


        private static string GetFolderName() {
            return typeof(T).Name switch {
                nameof(ReportImage) => "reports",
                nameof(CleanupImage) => "cleanups",
                nameof(WasteScanImage) => "WasteScanImages",
                nameof(ProductImage) => "ProductImage",
                nameof(UserProfileImage) => "UserProfiles",
                _ => "others"
            };
        }

        private static void SetOwner(T image, int ownerId) {
            switch (image) {
                case ReportImage reportImage:
                reportImage.ReportId = ownerId;
                break;
                case CleanupImage cleanupImage:
                cleanupImage.CleanupTaskId = ownerId;
                break;
                case WasteScanImage wasteScanImage:
                wasteScanImage.AIWasteScanId = ownerId;
                break;
                case ProductImage productImage:
                productImage.ProductId = ownerId;
                break;
                case UserProfileImage userProfileImage:
                userProfileImage.UserId = ownerId;
                break;
                default:
                throw new InvalidOperationException("Unsupported image type");
            }
        }
    }

}
