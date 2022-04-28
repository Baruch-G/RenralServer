using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rental.Common.Settings;
using System;
using System.Reflection;

namespace Rental.Common.Extensions
{ 
    public static class Extensions{
        public static IServiceCollection AddMassTransitWithRabbitMQ(this IServiceCollection services){
             services.AddMassTransit(x => {
                 x.AddConsumers(Assembly.GetEntryAssembly());
                x.UsingRabbitMq((context, configurator) => {
                    var configuration = context.GetService<IConfiguration>();
                    var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                    var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                    configurator.Host(rabbitMQSettings.Host);
                    configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                    configurator.UseMessageRetry(r => {
                        r.Interval(3, TimeSpan.FromSeconds(5));
                    });
                });
            });
            return services;
        }
    }
}