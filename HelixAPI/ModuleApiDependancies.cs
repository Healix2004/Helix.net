using Helix.Core;
using Microsoft.OpenApi.Models;

namespace Helix.API
{
    public static class ModuleApiDependancies
    {
        public static IServiceCollection AddApiDenpendancies(this IServiceCollection services, IConfiguration configuration = null)
        {
            services.AddControllers();
            services.AddCoreDependencies(); // Register MediatR, Validators, etc.
            services.AddSwagger();
            services.AddCors(configuration);
            return services;
        }

        private static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });

                // For production, use specific origins from configuration
                if (configuration != null)
                {
                    var allowedOrigins = configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();
                    if (allowedOrigins != null && allowedOrigins.Length > 0)
                    {
                        options.AddPolicy("AllowSpecificOrigins", policy =>
                        {
                            policy.WithOrigins(allowedOrigins)
                                  .AllowAnyMethod()
                                  .AllowAnyHeader()
                                  .AllowCredentials();
                        });
                    }
                }
            });
            return services;
        }

        private static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by space and your JWT token."
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            return services;
        }
    }
}
