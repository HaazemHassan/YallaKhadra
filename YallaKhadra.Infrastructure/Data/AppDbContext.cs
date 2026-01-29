using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using YallaKhadra.Core.Entities.BaseEntities;
using YallaKhadra.Core.Entities.E_CommerceEntities;
using YallaKhadra.Core.Entities.GreenEntities;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Entities.PointsEntities;

namespace YallaKhadra.Infrastructure.Data {
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>> {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<WasteReport> Reports { get; set; }
        public DbSet<BaseImage> Images { get; set; }
        public DbSet<ReportImage> ReportImages { get; set; }
        public DbSet<PointsTransaction> PointsTransactions { get; set; }
        public DbSet<CleanupTask> CleanupTasks { get; set; }
        public DbSet<CleanupImage> CleanupImages { get; set; }
        public DbSet<AIWasteScan> AIWasteScans { get; set; }
        public DbSet<WasteScanImage> WasteScanImages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderShippingDetails> OrderShippingDetails { get; set; }
        public DbSet<UserProfileImage> UserProfileImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("AspNetUserClaims", schema: "identity");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("AspNetUserRoles", schema: "identity");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("AspNetUserLogins", schema: "identity");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("AspNetRoleClaims", schema: "identity");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("AspNetUserTokens", schema: "identity");
        }
    }
}
