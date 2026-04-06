using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Options;
using YallaKhadra.Services.Emails;
using YallaKhadra.Services.Services;
using YallaKhadra.Services.Services.Ecommerce_services;

namespace YallaKhadra.Services {
    public static class ServicesDependencyRegisteration {
        public static IServiceCollection ServiceLayerDependencyRegistration(this IServiceCollection services, IConfiguration configuration) {
            services.Configure<MailOptions>(configuration.GetSection(MailOptions.SectionName));
            services.Configure<VerificationCodeOptions>(configuration.GetSection(VerificationCodeOptions.SectionName));

            services.AddTransient<IApplicationUserService, ApplicationUserService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IEmailVerificationService, EmailVerificationService>();
            services.AddTransient<IPasswordService, PasswordService>();
            services.AddSingleton<IOtpService, OtpService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddSingleton<IEmailBodyBuilderService, EmailBodyBuilderService>();
            services.AddTransient<IWasteReportService, WasteReportService>();
            services.AddTransient<IPointsTransactionService, PointsTransactionService>();
            services.AddTransient<ICleanupTaskService, CleanupTaskService>();
            services.AddTransient<IAIWasteScanService, AIWasteScanService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<ICloudinaryService, CloudinaryService>();
            services.AddTransient(typeof(IImageService<>), typeof(ImageService<>));

            return services;
        }
    }
}
