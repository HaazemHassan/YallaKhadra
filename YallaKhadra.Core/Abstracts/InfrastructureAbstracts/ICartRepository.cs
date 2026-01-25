using YallaKhadra.Core.Entities.E_CommerceEntities;

namespace YallaKhadra.Core.Abstracts.InfrastructureAbstracts {
    public interface ICartRepository : IGenericRepository<Cart> {
        Task<Cart?> GetCartByUserIdWithItemsAsync(int userId, CancellationToken cancellationToken = default);
    }
}
