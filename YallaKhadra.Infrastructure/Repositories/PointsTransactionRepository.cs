using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Entities;
using YallaKhadra.Infrastructure.Data;

namespace YallaKhadra.Infrastructure.Repositories
{
    public class PointsTransactionRepository : GenericRepository<PointsTransaction>, IPointsTransactionRepository
    {
        private readonly DbSet<PointsTransaction> _pointsTransaction;
        public PointsTransactionRepository(AppDbContext context) : base(context)
        {
            _pointsTransaction = context.Set<PointsTransaction>();
        }
    }
}
