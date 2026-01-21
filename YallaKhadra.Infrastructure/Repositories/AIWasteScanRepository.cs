using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Entities;
using YallaKhadra.Infrastructure.Data;

namespace YallaKhadra.Infrastructure.Repositories
{
    public class AIWasteScanRepository : GenericRepository<AIWasteScan>, IAIWasteScanRepository
    {
        private readonly DbSet<AIWasteScan> _aIWasteScan;


        public AIWasteScanRepository(AppDbContext context) : base(context)
        {
            _aIWasteScan = context.Set<AIWasteScan>();
        }
    }
}
