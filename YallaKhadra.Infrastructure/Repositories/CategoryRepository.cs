using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Entities.E_CommerceEntities;
using YallaKhadra.Infrastructure.Data;

namespace YallaKhadra.Infrastructure.Repositories {
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository {
        private readonly DbSet<Category> _categories;

        public CategoryRepository(AppDbContext context) : base(context) {
            _categories = context.Set<Category>();
        }
    }
}
