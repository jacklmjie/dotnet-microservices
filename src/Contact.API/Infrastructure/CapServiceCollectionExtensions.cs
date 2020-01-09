using Contact.API.Event;
using DotNetCore.CAP.Dashboard.NodeDiscovery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Contact.API.Infrastructure
{
    public static class CapServiceCollectionExtensions
    {
        public static IServiceCollection AddMyCap(this IServiceCollection services,
          IConfigurationSection section)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            services.Configure<CapOptions>(section);
            services.AddTransient<ISubscriberService, SubscriberService>();

            var options = section.Get<CapOptions>();
            services.AddCap(x =>
            {
                x.UseMySql(options.MySql);
                x.UseRabbitMQ(options.RabbitMQ);
                x.UseDashboard();
                x.UseDiscovery(d =>
                {
                    d.DiscoveryServerHostName = options.DiscoveryServerHostName;
                    d.DiscoveryServerPort = options.DiscoveryServerPort;
                    d.CurrentNodeHostName = options.CurrentNodeHostName;
                    d.CurrentNodePort = options.CurrentNodePort;
                    d.NodeId = options.NodeId;
                    d.NodeName = options.NodeName;
                });
            });
            return services;
        }
    }
}
