using AutoMapper;
using Helix.Data.Entities;
using Helix.Infrastructure.Context;
using Helix.Infrastructure.Context.DbInitializer;
using Helix.Infrastructure.ExternalServices;
using Helix.Infrastructure.Seeding;
using Helix.Service.Interfaces;
using Helix.Service.Repositories;
using Helix.Service.Services;
using Helix.Service.Services.AllergenCatalogServices;
using Helix.Service.Services.AppointmentService;
using Helix.Service.Services.AuthServices;
using Helix.Service.Services.ChronicDiseaseCatalogServices;
using Helix.Service.Services.ConsentService;
using Helix.Service.Services.DrugDataService;
using Helix.Service.Services.EmergencyAccessService;
using Helix.Service.Services.FileServices;
using Helix.Service.Services.LoincTerminology;
using Helix.Service.Services.MedicalConceptCatalogServices;
using Helix.Service.Services.MedicationCatalogServices;
using Helix.Service.Services.NotificationService;
using Helix.Service.Services.ProcedureCatalogServices;
using Helix.Service.Services.RadiologyOrderService;
using Helix.Service.Services.RxNavTerminology;
using Helix.Service.Services.SnowstormTerminology;
using Helix.Service.Services.SpecialtyCatalogServices;
using Helix.Service.Services.TokenProvider;
using Helix.Service.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace Helix.Service
{
    public static class ModuleServiceDependancies
    {
        public static IServiceCollection AddServiceDependancies(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddDbContext(configuration, env);
            services.AddDbInitializer();
            services.AddLoincSeeder();
            services.AddLoincPanelSeeder();
            services.AddIdentity();
            services.AddJWT(configuration, env);
            services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
            services.AddAuth();
            // add application services
            services.AddFileService();
            services.AddEmailService();
            services.AddDrugService();
            services.AddPatientService();
            services.AddDoctorService();
            services.AddConsentService();
            services.AddConsentValidationService();
            services.AddAllergyService();
            services.AddDiagnoseService();
            services.AddEncounterService();
            services.AddEmergencyAccessService();
            services.AddFacilitieService();
            services.AddMedicationService();
            services.AddObservationService();
            services.AddRadiologyOrderService();
            services.AddAppointmentService();
            services.AddRadiologyResultService();
            services.AddLabOrderService();
            services.AddLabTestResultService();
            services.AddTerminologyCodeLookupService();
            services.AddUnitOfWork();
            services.AddNotificationService();
            services.AddLoincService(configuration);
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddMemoryCache();
            services.AddAllergenCatalogService();
            services.AddChronicDiseaseCatalogService();
            services.AddChronicDiseaseCatalogService();
            services.AddProcedureCatalogService();
            services.AddMedicationCatalogService();
            services.AddSpecialtyCatalogService();
            services.AddMedicalConceptCatalogService();

            services.AddScoped<ITerminologyService, RemoteFhirTerminologyService>();
            services.AddHttpClient<IRxNavTerminologyService, RxNavTerminologyService>(client =>
            {
                // Set the base address for the RxNav API
                client.BaseAddress = new Uri("https://rxnav.nlm.nih.gov/REST/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddHttpClient<ISnowstormTerminologyService, SnowstormTerminologyService>(client =>
            {
                client.BaseAddress = new Uri("https://snowstorm.snomedtools.org/fhir/");
            });


            return services;
        }
        private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            string connectionString;
            {
                connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection String Not Found");
            }
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString,sqlServerOptionsAction: sqlOptions =>
            {
                // 1. Tell EF Core to automatically retry if the connection drops!
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);

                // 2. Give SQL Server more time to think during massive data seeds (e.g., 3 minutes)
                sqlOptions.CommandTimeout(180);
            }));
            return services;
        }
        private static IServiceCollection AddDbInitializer(this IServiceCollection services)
        {
            services.AddScoped<IDbInitializer, DbInitializer>();
            return services;
        }
        private static IServiceCollection AddLoincSeeder(this IServiceCollection services)
        {
            services.AddScoped<LoincSeeder>();
            return services;
        }
        private static IServiceCollection AddLoincPanelSeeder(this IServiceCollection services)
        {
            services.AddScoped<LoincPanelSeeder>();
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
                // This tells Identity to use the 6-digit provider for email confirmation
                option.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
                // Tell Identity to use the 6-digit email token provider for password resets
                option.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
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
        private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
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

        private static IServiceCollection AddPatientService(this IServiceCollection services)
        {
            services.AddScoped<IPatientService, Helix.Service.Services.PatientService.PatientService>();
            return services;
        }

        private static IServiceCollection AddDoctorService(this IServiceCollection services)
        {
            services.AddScoped<IDoctorService, Helix.Service.Services.DoctorService.DoctorService>();
            return services;
        }

        private static IServiceCollection AddConsentService(this IServiceCollection services)
        {
            services.AddScoped<IConsentService, Helix.Service.Services.ConsentService.ConsentService>();
            return services;
        }
        private static IServiceCollection AddConsentValidationService(this IServiceCollection services)
        {
            services.AddScoped<IConsentValidationService, ConsentValidationService>();
            return services;
        }
        private static IServiceCollection AddAllergyService(this IServiceCollection services)
        {
            services.AddScoped<IAllergyService, Helix.Service.Services.AllergyService.AllergyService>();
            return services;
        }

        private static IServiceCollection AddDiagnoseService(this IServiceCollection services)
        {
            services.AddScoped<IDiagnoseService, Helix.Service.Services.DiagnoseService.DiagnoseService>();
            return services;
        }

        private static IServiceCollection AddEncounterService(this IServiceCollection services)
        {
            services.AddScoped<IEncounterService, Helix.Service.Services.EncounterService.EncounterService>();
            return services;
        }

        private static IServiceCollection AddFacilitieService(this IServiceCollection services)
        {
            services.AddScoped<IFacilitieService, Helix.Service.Services.FacilitieService.FacilitieService>();
            return services;
        }

        private static IServiceCollection AddMedicationService(this IServiceCollection services)
        {
            services.AddScoped<IMedicationService, Helix.Service.Services.MedicationService.MedicationService>();
            return services;
        }

        private static IServiceCollection AddObservationService(this IServiceCollection services)
        {
            services.AddScoped<IObservationService, Helix.Service.Services.ObservationService.ObservationService>();
            return services;
        }

        private static IServiceCollection AddTerminologyCodeLookupService(this IServiceCollection services)
        {
            services.AddScoped<ITerminologyCodeLookupService, Helix.Service.Services.TerminologyCodeLookupService.TerminologyCodeLookupService>();
            return services;
        }
        private static IServiceCollection AddRadiologyOrderService(this IServiceCollection services)
        {
            services.AddScoped<IRadiologyOrderService, RadiologyOrderService>();
            return services;
        }
        private static IServiceCollection AddAppointmentService(this IServiceCollection services)
        {
            services.AddScoped<IAppointmentService, AppointmentService>();
            return services;
        }
        private static IServiceCollection AddRadiologyResultService(this IServiceCollection services)
        {
            services.AddScoped<IRadiologyResultService, Helix.Service.Services.RadiologyResultService.RadiologyResultService>();
            return services;
        }
        private static IServiceCollection AddAllergenCatalogService(this IServiceCollection services)
        {
            services.AddScoped < IAllergenCatalogService, AllergenCatalogService>();
            return services;
        }
        private static IServiceCollection AddChronicDiseaseCatalogService(this IServiceCollection services)
        {
            services.AddScoped<IChronicDiseaseCatalogService, ChronicDiseaseCatalogService>();
            return services;
        }
        private static IServiceCollection AddProcedureCatalogService(this IServiceCollection services)
        {
            services.AddScoped<IProcedureCatalogService, ProcedureCatalogService>();
            return services;
        }
        private static IServiceCollection AddMedicationCatalogService(this IServiceCollection services)
        {
            services.AddScoped<IMedicationCatalogService, MedicationCatalogService>();
            return services;
        }
        private static IServiceCollection AddSpecialtyCatalogService(this IServiceCollection services)
        {
            services.AddScoped<ISpecialtyCatalogService, SpecialtyCatalogService>();
            return services;
        }
        private static IServiceCollection AddMedicalConceptCatalogService(this IServiceCollection services)
        {
            services.AddScoped<IMedicalConceptCatalogService, MedicalConceptCatalogService>();
            return services;
        }
        private static IServiceCollection AddLabTestResultService(this IServiceCollection services)
        {
            services.AddScoped<ILabTestResultService, Helix.Service.Services.LabTestResultService.LabTestResultService>();
            return services;
        }
        private static IServiceCollection AddLabOrderService (this IServiceCollection services)
        {
            services.AddScoped<ILabOrderService, Helix.Service.Services.LabOrderService.LabOrderService>();
            return services;
        }
        private static IServiceCollection AddNotificationService(this IServiceCollection services)
        {
            services.AddScoped<INotificationService,NotificationService>();
            return services;
        }
        private static IServiceCollection AddEmergencyAccessService(this IServiceCollection services)
        {
            services.AddScoped<IEmergencyAccessService, EmergencyAccessService>();
            return services;
        }
        private static IServiceCollection AddLoincService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<ILoincTerminologyService, LoincTerminologyService>(client =>
            {
                client.BaseAddress = new Uri("https://fhir.loinc.org/");

                // Retrieve LOINC API credentials from configuration
                var username = configuration["LoincApi:Username"];
                var password = configuration["LoincApi:Password"];

                // Only add Basic Auth if credentials are provided
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Basic", Convert.ToBase64String(byteArray));
                }

                // Add default headers
                client.DefaultRequestHeaders.Add("Accept", "application/fhir+json");
            });

            return services;
        }
    }
}
