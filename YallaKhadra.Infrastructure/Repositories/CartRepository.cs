using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Entities.E_CommerceEntities;
using YallaKhadra.Infrastructure.Data;

namespace YallaKhadra.Infrastructure.Repositories {
    public class CartRepository : GenericRepository<Cart>, ICartRepository {
        private readonly DbSet<Cart> _carts;

        public CartRepository(AppDbContext context) : base(context) {
            _carts = context.Set<Cart>();
        }

        public async Task<Cart?> GetCartByUserIdWithItemsAsync(int userId, CancellationToken cancellationToken = default) {
            return await _carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        }
    }
}
