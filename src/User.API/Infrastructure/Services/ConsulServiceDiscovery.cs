using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.API.Extensions;

namespace User.API.Infrastructure.Services
{
    public class ConsulServiceDiscovery : IServiceDiscovery
    {
        private const string VERSION_PREFIX = "version-";

        private readonly ConsulClient _consul;

        public ConsulServiceDiscovery(ConsulClient consul)
        {
            _consul = consul;
        }

        private string GetVersionFromStrings(IEnumerable<string> strings)
        {
            return strings
                ?.FirstOrDefault(x => x.StartsWith(VERSION_PREFIX, StringComparison.Ordinal))
                .TrimStart(VERSION_PREFIX);
        }

        public async Task<IList<ServiceInformation>> FindServiceInstancesAsync(string name)
        {
            var queryResult = await _consul.Health.Service(name, tag: "", passingOnly: true);
            var instances = queryResult.Response.Select(serviceEntry => new ServiceInformation
            {
                Name = serviceEntry.Service.Service,
                HostAndPort = new HostAndPort(serviceEntry.Service.Address, serviceEntry.Service.Port),
                Version = GetVersionFromStrings(serviceEntry.Service.Tags),
                Tags = serviceEntry.Service.Tags ?? Enumerable.Empty<string>(),
                Id = serviceEntry.Service.ID
            });

            return instances.ToList();
        }
    }
}
