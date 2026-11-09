using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.Behavior;
using MyApp.Application.Features.Registration;

namespace MyApp.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            var assembly = typeof(UserRegistrationCommand).Assembly;

            // Register MediatR and Validation Pipeline
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            // Register all FluentValidation validators
            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
}