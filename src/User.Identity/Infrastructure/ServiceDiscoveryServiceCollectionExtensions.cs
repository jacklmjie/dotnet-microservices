using DnsClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace User.Identity.Infrastructure
{
    /// <summary>
    /// Consul服务注册与发现
    /// </summary>
    public static class ServiceDiscoveryServiceCollectionExtensions
    {
        public static IServiceCollection AddMyServiceDiscovery(this IServiceCollection services,
            Action<ServiceDiscoveryOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.Configure(setupAction);
            services.AddSingleton<IDnsQuery>(p =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<ServiceDiscoveryOptions>>().Value;
                return new LookupClient(serviceConfiguration.Consul.DnsEndpoint.ToIPEndPoint());
            });

            return services;
        }
    }
}
