using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Entities.E_CommerceEntities;
using YallaKhadra.Infrastructure.Data;

namespace YallaKhadra.Infrastructure.Repositories {
    public class ProductRepository : GenericRepository<Product>, IProductRepository {
        private readonly DbSet<Product> _products;

        public ProductRepository(AppDbContext context) : base(context) {
            _products = context.Set<Product>();
        }
    }
}
