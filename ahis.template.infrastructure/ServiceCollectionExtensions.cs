using ahis.template.application.Shared.Mediator;
using ahis.template.application.Interfaces.Repositories;
using ahis.template.infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ahis.template.infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {

            // Register repositories
            services.AddScoped<ICountryRepository, CountryRepository>();

            // Register custom Mediator
            services.AddScoped<IMediator, SimpleMediator>();

            // Automatically register all IRequestHandler<,> handlers
            var handlerType = typeof(IRequestHandler<,>);

            // Assembly that contains handlers
            var applicationAssembly = Assembly.Load("ahis.template.application");

            foreach (var type in applicationAssembly.GetTypes())
            {
                var interfaces = type.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType);

                foreach (var @interface in interfaces)
                {
                    services.AddScoped(@interface, type);
                }
            }

            return services;
        }
    }
}
