using Microsoft.AspNetCore.Http;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities.E_CommerceEntities;

namespace YallaKhadra.Core.Abstracts.ServicesContracts {
    public interface IProductService {
        Task<ServiceOperationResult<Product>> CreateAsync(
            Product product,
            List<IFormFile> images,
            int uploadedBy,
            CancellationToken cancellationToken = default);

        Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<ServiceOperationResult<Product>> UpdateAsync(
            Product product,
            List<IFormFile>? images,
            int updatedBy,
            CancellationToken cancellationToken = default);

        Task<ServiceOperationResult<Product>> DeleteAsync(
            int productId,
            CancellationToken cancellationToken = default);
    }
}
