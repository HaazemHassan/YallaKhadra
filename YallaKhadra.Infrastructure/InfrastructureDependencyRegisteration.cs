using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Infrastructure.BackgroundJobs;
using YallaKhadra.Infrastructure.BackgroundJobs.Jobs;
using YallaKhadra.Infrastructure.Data;
using YallaKhadra.Infrastructure.Repositories;

namespace YallaKhadra.Infrastructure;

public static class InfrastructureDependencyRegisteration {

    public static IServiceCollection InfrastrctureLayerDepenedencyRegistration(this IServiceCollection services, IConfiguration configuration) {

        DbContextServiceConfiguations(services, configuration);
        RepositoryServiceConfiguations(services);
        IdentityServiceConfiguations(services);
        HangfireServiceConfiguations(services, configuration);
        BackgroundJobsServiceConfiguations(services);
        return services;

    }


    private static IServiceCollection DbContextServiceConfiguations(IServiceCollection services, IConfiguration configuration) {
        services.AddDbContext<AppDbContext>(options => {
            options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);
        });

        return services;
    }


    private static IServiceCollection IdentityServiceConfiguations(IServiceCollection services) {

        services.AddIdentity<ApplicationUser, ApplicationRole>(option => {
            // Password settings.
            option.Password.RequireDigit = false;
            option.Password.RequireLowercase = false;
            option.Password.RequireNonAlphanumeric = false;
            option.Password.RequireUppercase = false;
            option.Password.RequiredLength = 3;
            option.Password.RequiredUniqueChars = 1;

            // Lockout settings.
            option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            option.Lockout.MaxFailedAccessAttempts = 5;
            option.Lockout.AllowedForNewUsers = true;

            // User settings.
            option.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            option.User.RequireUniqueEmail = true;
            option.SignIn.RequireConfirmedEmail = false;


        }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

        return services;
    }


    private static IServiceCollection RepositoryServiceConfiguations(this IServiceCollection services) {
        services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddTransient<IVerificationCodeRepository, VerificationCodeRepository>();
        services.AddTransient<IWasteReportRepository, WasteReportRepository>();
        services.AddTransient<IPointsTransactionRepository, PointsTransactionRepository>();
        services.AddTransient<ICleanupTaskRepository, CleanupTaskRepository>();
        services.AddTransient<IAIWasteScanRepository, AIWasteScanRepository>();
        services.AddTransient<IProductRepository, ProductRepository>();
        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddTransient<ICartRepository, CartRepository>();
        services.AddTransient<ICartItemRepository, CartItemRepository>();
        services.AddTransient<IOrderRepository, OrderRepository>();




        return services;
    }

    private static IServiceCollection HangfireServiceConfiguations(IServiceCollection services, IConfiguration configuration) {
        services.Configure<HangfireOptions>(configuration.GetSection(HangfireOptions.SectionName));

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

        services.AddHangfireServer();
        return services;
    }

    private static IServiceCollection BackgroundJobsServiceConfiguations(IServiceCollection services) {
        services.AddTransient<SendEmailJob>();
        return services;
    }
}
