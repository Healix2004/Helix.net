using Helix.Data.Entities;
using Helix.Infrastructure.Context;
using Helix.Infrastructure.Context.DbInitializer;
using Helix.Infrastructure.ExternalServices;
using Helix.Service.Interfaces;
using Helix.Service.Services;
using Helix.Service.Services.AuthServices;
using Helix.Service.Services.DrugDataService;
using Helix.Service.Services.FileServices;
using Helix.Service.Services.TerminologyServices;
using Helix.Service.Services.TokenProvider;
using Helix.Service.Settings;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Helix.Service
{
    public static class ModuleServiceDependancies
    {
        public static IServiceCollection AddServiceDependancies(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddDbContext(configuration, env);
            services.AddDbInitializer();
            services.AddIdentity();
            services.AddJWT(configuration, env);
            services.AddAutoMapper(typeof(ModuleServiceDependancies));
            services.AddAuth();
            services.AddFileService();
            services.AddEmailService();
            services.AddDrugService();
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            services.AddScoped<ITerminologyService, RemoteFhirTerminologyService>();

            return services;
        }
        private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            string connectionString;
            {
                connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection String Not Found");
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
            return services;
        }
        private static IServiceCollection AddDbInitializer(this IServiceCollection services)
        {
            services.AddScoped<IDbInitializer, DbInitializer>();
            return services;
        }
        private static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole>(option =>
            {
                // Password settings.
                option.Password.RequireDigit = true;
                option.Password.RequireLowercase = true;
                option.Password.RequireNonAlphanumeric = true;
                option.Password.RequireUppercase = true;
                option.Password.RequiredLength = 8;
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

            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            return services;
        }
        private static IServiceCollection AddJWT(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.IncludeErrorDetails = env.IsDevelopment();
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
                    if (jwtSettings == null)
                    {
                        throw new InvalidOperationException("JWT settings not found in configuration.");
                    }
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddScoped<ITokenProvider, JwtTokeProvider>();

            return services;
        }
        private static IServiceCollection AddAuth(this IServiceCollection services)
        {
            services.AddTransient<IAuthService, AuthService>();
            return services;
        }

        private static IServiceCollection AddFileService(this IServiceCollection services)
        {
            services.AddTransient<IFileService, FileService>();
            return services;
        }
        private static IServiceCollection AddEmailService(this IServiceCollection services)
        {
            services.AddScoped<IEmailService, EmailServices>();
            return services;
        }
        private static IServiceCollection AddDrugService(this IServiceCollection services)
        {
            services.AddSingleton<IDrugDataService, DrugDataService>();
            return services;
        }
        //private static IServiceCollection AddFhirSerialization(this IServiceCollection services)
        //{
        //    // FHIR Serialization settings can be configured here if needed
        //    // Create a reusable serializer
        //    services.AddSingleton<FhirJsonSerializer>(new FhirJsonSerializer(new SerializerSettings()
        //    {
        //        Pretty = true,
        //        AppendFormToTargets = false
        //    }));
        //    return services;
        //}
    }
}
