using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.API.Bases.DataSeeding;
using YallaKhadra.API.Extentions;
using YallaKhadra.API.Middlewares;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Infrastructure.Data;

namespace YallaKhadra.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();
            builder.Services.DependenciesRegistration(builder.Configuration);
            //builder.Services.AddCors(options => {
            //    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            //                         ?? new[] {
            //                             "http://localhost:4200",
            //                             "https://localhost:4200",
            //                             "http://localhost:4201",
            //                             "https://localhost:4201"
            //                         };

            //    options.AddPolicy("AngularClientPolicy", policy => {
            //        policy.WithOrigins(allowedOrigins)
            //              .AllowAnyHeader()
            //              .AllowAnyMethod();
            //    });
            //});

            builder.Services.AddCors(options => {
                options.AddPolicy("AngularClientPolicy", policy => {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });


            var app = builder.Build();

            #region Initialize Database
            using (var scope = app.Services.CreateScope())
            {

                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // needed to dockerize the application and have the DB created automatically       
                if (app.Environment.IsDevelopment())
                {
                    try
                    {
                        await context.Database.MigrateAsync();
                    }
                    catch (SqlException ex) when (ex.Number == 2714)
                    {
                        Console.WriteLine("Tables already exist, skipping migration");
                    }
                    catch (Exception ex)
                    {
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

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI();

                var dashboardPath = app.Configuration["HangfireSettings:DashboardPath"] ?? "/jobs";
                app.UseHangfireDashboard(dashboardPath);
            }

            app.UseCors("AngularClientPolicy");


            app.UseErrorHandling();
            app.UseForwardedHeaders();   // Use Forwarded Headers (must be early in pipeline)
            app.UseSecurityHeaders();
            //app.UseHttpsRedirection();
            app.UseGuestSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();    //must be after UseAuthentication and UseAuthorization because we are using user identity name in rate limiting policy
            app.MapControllers();

            app.Run();
        }
    }
}
