using YallaKhadra.Core.Entities.E_CommerceEntities;

namespace YallaKhadra.Core.Abstracts.InfrastructureAbstracts {
    public interface IOrderRepository : IGenericRepository<Order> {
        Task<List<Order>> GetOrdersByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    }
}
