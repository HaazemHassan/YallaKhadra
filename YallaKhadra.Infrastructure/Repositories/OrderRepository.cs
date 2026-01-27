using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Entities.E_CommerceEntities;
using YallaKhadra.Infrastructure.Data;

namespace YallaKhadra.Infrastructure.Repositories {
    public class OrderRepository : GenericRepository<Order>, IOrderRepository {
        private readonly DbSet<Order> _orders;

        public OrderRepository(AppDbContext context) : base(context) {
            _orders = context.Set<Order>();
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(int userId, CancellationToken cancellationToken = default) {
            return await _orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingDetails)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }
    }
}
