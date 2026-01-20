using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Services.Services;

namespace YallaKhadra.Services {
    public static class ServicesDependencyRegisteration {
        public static IServiceCollection ServiceLayerDependencyRegistration(this IServiceCollection services, IConfiguration configuration) {
            services.AddTransient<IApplicationUserService, ApplicationUserService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IWasteReportService, WasteReportService>();
            services.AddTransient<IPointsTransactionService, PointsTransactionService>();
            services.AddTransient<ICleanupTaskService, CleanupTaskService>();
            services.AddTransient<ICloudinaryService, CloudinaryService>();
            services.AddTransient(typeof(IImageService<>), typeof(ImageService<>));

            return services;
        }
    }
}
