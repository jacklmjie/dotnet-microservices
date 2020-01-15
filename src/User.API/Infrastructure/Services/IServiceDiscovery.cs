using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace User.API.Infrastructure.Services
{
    public interface IServiceDiscovery
    {
        Task<IList<ServiceInformation>> FindServiceInstancesAsync(string name);
    }

    public class ServiceInformation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public HostAndPort HostAndPort { get; set; }
        public Uri ToUri(string scheme = "http", string path = "/")
        {
            var builder = new UriBuilder(scheme, HostAndPort.Address, HostAndPort.Port, path);
            return builder.Uri;
        }

        public override string ToString()
        {
            return $"{HostAndPort.Address}:{HostAndPort.Port}";
        }
    }

    public class HostAndPort
    {
        public HostAndPort(string downstreamHost, int downstreamPort)
        {
            Address = downstreamHost?.Trim('/');
            Port = downstreamPort;
        }

        public string Address { get; private set; }
        public int Port { get; private set; }
        public Uri ToUri(string scheme = "http", string path = "/")
        {
            var builder = new UriBuilder(scheme, this.Address, this.Port, path);
            return builder.Uri;
        }
        public override string ToString()
        {
            return $"{this.Address}:{this.Port}";
        }
    }
}
