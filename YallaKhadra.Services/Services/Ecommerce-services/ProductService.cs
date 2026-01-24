using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities.E_CommerceEntities;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.Services.Services.Ecommerce_services {
    public class ProductService : IProductService {
        private readonly IProductRepository _productRepository;
        private readonly IImageService<ProductImage> _imageService;

        public ProductService(
            IProductRepository productRepository,
            IImageService<ProductImage> imageService) {
            _productRepository = productRepository;
            _imageService = imageService;
        }

        public async Task<ServiceOperationResult<Product>> CreateAsync(
            Product product,
            List<IFormFile> images,
            int uploadedBy,
            CancellationToken cancellationToken = default) {
            if (images == null || images.Count != 3) {
                return ServiceOperationResult<Product>.Failure(
                    ServiceOperationStatus.Failed,
                    "Exactly 3 images are required")!;
            }


            /* AddAsync instead of AddWithoutSaveAsync to ensure the product is saved and has an Id
            because the id of this added product is needed when uploading images */
            var addedProduct = await _productRepository.AddAsync(product);

            if (addedProduct is not null) {
                for (int i = 0; i < images.Count; i++) {
                    var image = images[i];
                    if (image.Length > 0) {
                        var isMainImage = i == 0;

                        var uploadedImage = await _imageService.UploadWithoutSaveAsync(
                            image,
                            uploadedBy,
                            addedProduct.Id,
                            cancellationToken);

                        if (uploadedImage != null)
                            uploadedImage.IsMain = isMainImage;

                    }
                }
            }

            return ServiceOperationResult<Product>.Success(addedProduct)!;
        }

        public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default) {
            return await _productRepository
                .GetTableNoTracking(p => p.Id == id)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceOperationResult<Product>> UpdateAsync(
            Product product,
            List<IFormFile>? images,
            int updatedBy,
            CancellationToken cancellationToken = default) {

            var existingProduct = await _productRepository
                .GetTableAsTracking(p => p.Id == product.Id)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingProduct is null) {
                return ServiceOperationResult<Product>.Failure(
                    ServiceOperationStatus.NotFound,
                    "Product not found")!;
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.PointsCost = product.PointsCost;
            existingProduct.Stock = product.Stock;
            existingProduct.IsActive = product.IsActive;

            if (images != null && images.Count == 3) {
                var oldImages = existingProduct.Images.ToList();
                foreach (var oldImage in oldImages) {
                    await _imageService.DeleteAsync(oldImage);
                }

                for (int i = 0; i < images.Count; i++) {
                    var image = images[i];
                    if (image.Length > 0) {
                        var isMainImage = i == 0;

                        var uploadedImage = await _imageService.UploadWithoutSaveAsync(
                            image,
                            updatedBy,
                            existingProduct.Id,
                            cancellationToken);

                        if (uploadedImage != null)
                            uploadedImage.IsMain = isMainImage;
                    }
                }
            }

            await _productRepository.UpdateAsync(existingProduct);
            return ServiceOperationResult<Product>.Success(existingProduct)!;
        }

        public async Task<ServiceOperationResult<Product>> DeleteAsync(
            int productId,
            CancellationToken cancellationToken = default) {

            var product = await _productRepository
                .GetTableAsTracking(p => p.Id == productId)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(cancellationToken);

            if (product is null) {
                return ServiceOperationResult<Product>.Failure(
                    ServiceOperationStatus.NotFound,
                    "Product not found")!;
            }

            var images = product.Images.ToList();
            foreach (var image in images) {
                await _imageService.DeleteAsync(image);
            }

            await _productRepository.DeleteAsync(product);
            return ServiceOperationResult<Product>.Success(product)!;
        }
    }
}
