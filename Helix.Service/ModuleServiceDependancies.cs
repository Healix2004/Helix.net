using Helix.Data.Entities;
using Helix.Infrastructure.Context;
using Helix.Service.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Helix.Service
{
    public static class ModuleServiceDependancies
    {
        public static IServiceCollection AddServiceDependancies(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            services.AddDbContext(configuration);
            //services.AddDbInitializer();
            services.AddIdentity();
            //services.AddJWT(configuration, env);
            //services.AddAutoMapper(Assembly.GetExecutingAssembly());
            //services.AddAuth();
            //services.AddFileService();
            //services.AddPaperService();
            //services.AddEmailService();
            //services.AddCountryRepository();
            //services.Configure<PassportPhotoSettings>(configuration.GetSection("PassportPhotoSettings"));
            //services.Configure<PaperSettings>(configuration.GetSection("PaperSettings"));
            //services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            return services;
        }
        private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString;
            {
                connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection String Not Found");
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
            return services;
        }
        //private static IServiceCollection AddDbInitializer(this IServiceCollection services)
        //{
        //    services.AddScoped<IDbInitializer, DbInitializer>();
        //    return services;
        //}
        private static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            // Use AddIdentityCore and AddRoles if AddIdentity is not available
            services.AddIdentityCore<AppUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            /*.AddDefaultTokenProviders()*/;

            return services;
        }
        //private static IServiceCollection AddJWT(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        //{
        //    services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));
        //    services.AddAuthentication(options =>
        //    {
        //        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //    })
        //        .AddJwtBearer(options =>
        //        {
        //            options.IncludeErrorDetails = env.IsDevelopment();
        //            options.RequireHttpsMetadata = false;
        //            options.SaveToken = true;
        //            var jwtSettings = configuration.GetSection(JwtSettings.Section).Get<JwtSettings>();
        //            if (jwtSettings == null)
        //            {
        //                throw new InvalidOperationException("JWT settings not found in configuration.");
        //            }
        //            options.TokenValidationParameters = new TokenValidationParameters
        //            {
        //                ValidateIssuer = true,
        //                ValidateAudience = true,
        //                ValidateLifetime = true,
        //                ValidateIssuerSigningKey = true,
        //                ValidIssuer = jwtSettings.Issuer,
        //                ValidAudience = jwtSettings.Audience,
        //                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        //                ClockSkew = TimeSpan.Zero
        //            };
        //        });

        //    services.AddScoped<ITokenProvider, JwtTokeProvider>();

        //    return services;
        //}
        //private static IServiceCollection AddAuth(this IServiceCollection services)
        //{
        //    services.AddTransient<IAuthService, AuthService>();
        //    return services;
        //}
    }
}
