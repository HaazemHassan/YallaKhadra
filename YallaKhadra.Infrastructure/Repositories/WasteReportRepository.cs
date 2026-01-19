using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Entities;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Infrastructure.Data;

namespace YallaKhadra.Infrastructure.Repositories
{
    public class WasteReportRepository : GenericRepository<WasteReport> , IWasteReportRepository 
    {
        private readonly DbSet<WasteReport> _wasteReports;


        public WasteReportRepository(AppDbContext context) : base(context)
        {
            _wasteReports = context.Set<WasteReport>();
        }
    }
}
