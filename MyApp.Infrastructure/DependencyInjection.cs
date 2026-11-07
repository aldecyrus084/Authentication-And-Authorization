using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.Interfaces.Repository;
using MyApp.Application.Interfaces.Services;
using MyApp.Application.Services;
using MyApp.Infrastructure.Persistence;
using MyApp.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}
