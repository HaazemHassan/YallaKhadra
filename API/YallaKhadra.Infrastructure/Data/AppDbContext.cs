using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using YallaKhadra.Core.Entities;
using YallaKhadra.Core.Entities.IdentityEntities;

namespace YallaKhadra.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        //public DbSet<Conversation> Conversations { get; set; }

        public DbSet<Report> Reports { get; set; }
        public DbSet<Photo> photos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());



        }
    }
}
