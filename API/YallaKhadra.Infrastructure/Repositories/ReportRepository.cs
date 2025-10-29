using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Entities;
using YallaKhadra.Infrastructure.Data;

namespace YallaKhadra.Infrastructure.Repositories
{
    public class ReportRepository : GenericRepository<Report>, IReportRepository
    {
        private readonly DbSet<Report> _reports;


        public ReportRepository(AppDbContext context) : base(context)
        {
            _reports = context.Set<Report>();
        }
    }
}
