using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using YallaKhadra.Core.Entities;
using YallaKhadra.Core.Entities.IdentityEntities;

namespace YallaKhadra.Infrastructure.Data {
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>> {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<WasteReport> Reports { get; set; }
        public DbSet<BaseImage> Images { get; set; }
        public DbSet<ReportImage> ReportImages { get; set; }
        public DbSet<PointsTransaction> PointsTransactions { get; set; }
        public DbSet<CleanupTask> CleanupTasks { get; set; }
        public DbSet<CleanupImage> CleanupImages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }
    }
}
