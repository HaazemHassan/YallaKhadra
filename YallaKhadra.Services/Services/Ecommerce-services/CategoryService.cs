using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities.E_CommerceEntities;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.Services.Services.Ecommerce_services {
    public class CategoryService : ICategoryService {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;

        public CategoryService(ICategoryRepository categoryRepository, IProductRepository productRepository) {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }

        public async Task<ServiceOperationResult<Category>> CreateAsync(
            Category category,
            CancellationToken cancellationToken = default) {

            var existingCategory = await _categoryRepository
                .GetTableNoTracking(c => c.Name == category.Name)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingCategory is not null) {
                return ServiceOperationResult<Category>.Failure(
                    ServiceOperationStatus.AlreadyExists,
                    "Category with this name already exists.")!;
            }

            var createdCategory = await _categoryRepository.AddAsync(category);
            return ServiceOperationResult<Category>.Success(createdCategory)!;
        }

        public async Task<ServiceOperationResult<Category>> UpdateAsync(
            int categoryId,
            string? name,
            string? description,
            CancellationToken cancellationToken = default) {

            var category = await _categoryRepository
                .GetTableAsTracking(c => c.Id == categoryId)
                .FirstOrDefaultAsync(cancellationToken);

            if (category is null) {
                return ServiceOperationResult<Category>.Failure(
                    ServiceOperationStatus.NotFound,
                    "Category not found.")!;
            }

            if (!string.IsNullOrWhiteSpace(name)) {
                var existingCategoryWithName = await _categoryRepository
                    .GetTableNoTracking(c => c.Name == name && c.Id != categoryId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (existingCategoryWithName is not null) {
                    return ServiceOperationResult<Category>.Failure(
                        ServiceOperationStatus.AlreadyExists,
                        "Another category with this name already exists.")!;
                }

                category.Name = name;
            }

            if (!string.IsNullOrWhiteSpace(description))
                category.Description = description;


            await _categoryRepository.UpdateAsync(category);
            return ServiceOperationResult<Category>.Success(category)!;
        }

        public async Task<ServiceOperationResult<Category>> DeleteAsync(
            int categoryId,
            CancellationToken cancellationToken = default) {

            var category = await _categoryRepository
                .GetTableAsTracking(c => c.Id == categoryId)
                .FirstOrDefaultAsync(cancellationToken);

            if (category is null) {
                return ServiceOperationResult<Category>.Failure(
                    ServiceOperationStatus.NotFound,
                    "Category not found.")!;
            }

            bool hasProducts = await _productRepository
                .GetTableNoTracking()
                .AnyAsync(p => p.CategoryId == categoryId, cancellationToken);

            if (hasProducts) {
                return ServiceOperationResult<Category>.Failure(
                    ServiceOperationStatus.Failed,
                    "Cannot delete category that has products associated with it.")!;
            }

            await _categoryRepository.DeleteAsync(category);
            return ServiceOperationResult<Category>.Success(category)!;
        }
    }
}
