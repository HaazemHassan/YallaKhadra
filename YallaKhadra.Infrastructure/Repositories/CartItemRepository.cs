using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Entities.E_CommerceEntities;
using YallaKhadra.Infrastructure.Data;

namespace YallaKhadra.Infrastructure.Repositories {
    public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository {
        private readonly DbSet<CartItem> _cartItems;

        public CartItemRepository(AppDbContext context) : base(context) {
            _cartItems = context.Set<CartItem>();
        }
    }
}
