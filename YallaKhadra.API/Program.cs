using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.API.Bases.DataSeeding;
using YallaKhadra.API.Extentions;
using YallaKhadra.API.Middlewares;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Infrastructure.Data;

namespace YallaKhadra.API {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();
            builder.Services.DependenciesRegistration(builder.Configuration);


            var app = builder.Build();

            #region Initialize Database
            using (var scope = app.Services.CreateScope()) {

                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // needed to dockerize the application and have the DB created automatically       
                if (app.Environment.IsDevelopment()) {
                    try {
                        await context.Database.EnsureCreatedAsync();
                        await context.Database.MigrateAsync();
                    }
                    catch (SqlException ex) when (ex.Number == 2714) {
                        Console.WriteLine("Tables already exist, skipping migration");
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"Database migration error: {ex.Message}");
                        throw;
                    }
                }
                //


                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                await ApplicationRoleSeeder.SeedAsync(roleManager);
                await ApplicationUserSeeder.SeedAsync(userManager);

            }
            #endregion

            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();

                app.UseSwaggerUI();
            }

            app.UseErrorHandling();
            app.UseForwardedHeaders();   // Use Forwarded Headers (must be early in pipeline)
            app.UseSecurityHeaders();
            app.UseHttpsRedirection();
            app.UseGuestSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();    //must be after UseAuthentication and UseAuthorization because we are using user identity name in rate limiting policy
            app.MapControllers();

            app.Run();
        }
    }
}
