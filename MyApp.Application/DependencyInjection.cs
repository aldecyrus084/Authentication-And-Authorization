using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.Interfaces.Repository;
using MyApp.Application.Interfaces.Services;
using MyApp.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            return services;
        }
    }
}
