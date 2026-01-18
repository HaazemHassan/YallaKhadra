using Microsoft.AspNetCore.Identity;
using YallaKhadra.API.Bases.DataSeeding;
using YallaKhadra.API.Extentions;
using YallaKhadra.API.Middlewares;
using YallaKhadra.Core.Entities.IdentityEntities;

namespace YallaKhadra.API {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();
            builder.Services.DependenciesRegistration(builder.Configuration);


            var app = builder.Build();

            #region Initialize Database
            using (var scope = app.Services.CreateScope()) {
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
            app.UseRateLimiter();    //must be after UseAuthentication and UseAuthorization be cause we are using user identity name in rate limiting policy
            app.MapControllers();

            app.Run();
        }
    }
}
