using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities.E_CommerceEntities;

namespace YallaKhadra.Core.Abstracts.ServicesContracts {
    public interface ICategoryService {
        Task<ServiceOperationResult<Category>> CreateAsync(
            Category category,
            CancellationToken cancellationToken = default);

        Task<ServiceOperationResult<Category>> UpdateAsync(
            int categoryId,
            string? name,
            string? description,
            CancellationToken cancellationToken = default);

        Task<ServiceOperationResult<Category>> DeleteAsync(
            int categoryId,
            CancellationToken cancellationToken = default);
    }
}
