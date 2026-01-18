using Helix.Core.Bases;
using Helix.Core.Bases.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;

namespace Helix.Core
{
    public static class ModuleCoreDependancies
    {
        public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Register MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

            // Register FluentValidation validators
            services.AddValidatorsFromAssembly(assembly);

            // Register validation behavior pipeline
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // Register ResponseHandler
            services.AddScoped<ResponseHandler>();

            return services;
        }
    }
}
