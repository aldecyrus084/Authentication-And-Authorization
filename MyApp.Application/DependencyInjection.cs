using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.Features.Registration;
using MyApp.Application.Services;

namespace MyApp.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {

            // Register MediatR and Validation Pipeline
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(JwtSettings).Assembly);
            });


            return services;
        }
    }
}