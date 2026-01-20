using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Entities;
using YallaKhadra.Infrastructure.Data;

namespace YallaKhadra.Infrastructure.Repositories
{
    public class CleanupTaskRepository : GenericRepository<CleanupTask>, ICleanupTaskRepository
    {
        private readonly DbSet<CleanupTask> _cleanupTask;


        public CleanupTaskRepository(AppDbContext context) : base(context)
        {
            _cleanupTask = context.Set<CleanupTask>();
        }
    }
}
