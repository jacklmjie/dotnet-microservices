using DnsClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Contact.API.Infrastructure
{
    /// <summary>
    /// Consul服务注册与发现
    /// </summary>
    public static class ServiceDiscoveryServiceCollectionExtensions
    {
        public static IServiceCollection AddMyServiceDiscovery(this IServiceCollection services,
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

            services.Configure<ServiceDiscoveryOptions>(section);
            services.AddSingleton<IDnsQuery>(p =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<ServiceDiscoveryOptions>>().Value;
                return new LookupClient(serviceConfiguration.Consul.DnsEndpoint.ToIPEndPoint());
            });

            return services;
        }
    }
}
