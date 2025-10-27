using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Text;
using System.Threading.RateLimiting;
using YallaKhadra.API.Services;
using YallaKhadra.Core;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Bases.Authentication;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Infrastructure;
using YallaKhadra.Services;

namespace YallaKhadra.API.Extentions {
    public static class RegisterDependencies {
        public static IServiceCollection DependenciesRegistration(this IServiceCollection services, IConfiguration configuration) {
            //API Layer Dependency Registrations
            services.AddTransient<ICurrentUserService, CurrentUserService>();
            services.AddTransient<IClientContextService, ClientContextService>();

            //Other Layers Dependency Registrations
            services.InfrastrctureLayerDepenedencyRegistration(configuration);
            services.ServiceLayerDependencyRegistration(configuration);
            services.CoreLayerDependencyRegistration(configuration);



            //Service Configurations
            AuthenticationServiceConfiguations(services, configuration);
            SwaggerServiceConfiguations(services, configuration);
            //EmailServiceConfiguations(services, configuration);
            AutorizationServiceConfiguations(services, configuration);
            RateLimitingDependencyConfigurations(services, configuration);
            return services;
        }

        private static IServiceCollection AuthenticationServiceConfiguations(this IServiceCollection services, IConfiguration configuration) {
            //JWT Authentication
            var jwtSettings = new JwtSettings();
            configuration.GetSection(nameof(jwtSettings)).Bind(jwtSettings);
            services.AddSingleton(jwtSettings);

            //Google Authentication
            var googleAuthSettings = new GoogleAuthSettings();
            configuration.GetSection("GoogleAuth").Bind(googleAuthSettings);
            services.AddSingleton(googleAuthSettings);

            services.AddAuthentication(x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(x => {
               x.RequireHttpsMetadata = false;
               x.SaveToken = true;
               x.TokenValidationParameters = new TokenValidationParameters {
                   ValidateIssuer = jwtSettings.ValidateIssuer,
                   ValidIssuers = new[] { jwtSettings.Issuer },
                   ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                   ValidAudience = jwtSettings.Audience,
                   ValidateAudience = jwtSettings.ValidateAudience,
                   ValidateLifetime = jwtSettings.ValidateLifeTime,
                   ClockSkew = TimeSpan.FromMinutes(2)
               };


               x.Events = new JwtBearerEvents {
                   OnMessageReceived = context => {
                       var accessToken = context.Request.Query["access_token"];
                       var path = context.HttpContext.Request.Path;

                       if (!string.IsNullOrEmpty(accessToken) &&
                           path.StartsWithSegments("/chatHub")) {
                           context.Token = accessToken;
                       }

                       return Task.CompletedTask;
                   }
               };
           }

           );



            return services;
        }

        private static IServiceCollection SwaggerServiceConfiguations(this IServiceCollection services, IConfiguration configuration) {
            // Swagger Configuration
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "YallaKhadra API",
                    Version = "v1",
                    Description = "API for YallaKhadra application"
                });

                options.EnableAnnotations();

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath)) {
                    options.IncludeXmlComments(xmlPath);
                }

                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        //private static IServiceCollection EmailServiceConfiguations(this IServiceCollection services, IConfiguration configuration) {
        //    var emailSettings = new EmailSettings();
        //    configuration.GetSection("emailSettings").Bind(emailSettings);
        //    services.AddSingleton(emailSettings);
        //    return services;
        //}

        private static IServiceCollection AutorizationServiceConfiguations(this IServiceCollection services, IConfiguration configuration) {
            services.AddAuthorization(options => {
                options.AddPolicy("ResetPasswordPolicy", policy => {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("purpose", "reset-password");
                });
            });
            return services;
        }

        private static IServiceCollection RateLimitingDependencyConfigurations
            (this IServiceCollection services, IConfiguration configuration) {
            services.AddRateLimiter(options => {
                options.AddPolicy("defaultLimiter", httpContext => {
                    var clientContextService = httpContext.RequestServices.GetRequiredService<IClientContextService>();
                    var user = httpContext.User?.Identity?.Name;
                    string partitionKey;
                    if (!string.IsNullOrEmpty(user)) {
                        partitionKey = user;
                    }
                    else {
                        partitionKey = clientContextService.GetClientIpAddress(httpContext);
                    }

                    return RateLimitPartition.GetSlidingWindowLimiter(partitionKey, key => new SlidingWindowRateLimiterOptions {
                        Window = TimeSpan.FromMinutes(1),
                        PermitLimit = 90,
                        QueueLimit = 10,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        SegmentsPerWindow = 4
                    });
                });

                options.AddPolicy("loginLimiter", httpContext => {
                    var clientContextService = httpContext.RequestServices.GetRequiredService<IClientContextService>();
                    var partitionKey = clientContextService.GetClientIpAddress(httpContext);

                    return RateLimitPartition.GetSlidingWindowLimiter(partitionKey, key => new SlidingWindowRateLimiterOptions {
                        Window = TimeSpan.FromMinutes(1),
                        PermitLimit = 5,
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        SegmentsPerWindow = 4
                    });
                });

                options.OnRejected = async (context, token) => {
                    if (context.HttpContext.Response.HasStarted) {
                        return;
                    }

                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.ContentType = "application/json";

                    // Add Retry-After header
                    int retryAfterSeconds;

                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)) {
                        retryAfterSeconds = (int)retryAfter.TotalSeconds;
                    }
                    else {
                        var endpoint = context.HttpContext.GetEndpoint();
                        var rateLimiterAttribute = endpoint?.Metadata.GetMetadata<EnableRateLimitingAttribute>();
                        var policyName = rateLimiterAttribute?.PolicyName;

                        retryAfterSeconds = policyName switch {
                            "loginLimiter" => 60,
                            "defaultLimiter" => 60,
                            _ => 60
                        };
                    }
                    context.HttpContext.Response.Headers.RetryAfter = retryAfterSeconds.ToString();


                    var response = new Response<string> {
                        StatusCode = HttpStatusCode.TooManyRequests,
                        Message = "Too many requests. Please try again later.",
                        Succeeded = false
                    };

                    await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken: token);
                };

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });
            return services;
        }
    }
}
