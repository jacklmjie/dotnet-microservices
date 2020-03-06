using DnsClient;
using System.Linq;

namespace Recommend.API.Infrastructure.Services
{
    public class ServiceDiscovery : IServiceDiscovery
    {
        private readonly IDnsQuery _dnsQuery;

        public ServiceDiscovery(IDnsQuery dnsQuery)
        {
            _dnsQuery = dnsQuery;
        }

        public string FindServiceInstances(string name)
        {
            //todo:负载均衡
            var address = _dnsQuery.ResolveService("service.consul", name);
            var addressList = address.First().AddressList;
            var host = addressList.Any() ?
                addressList.First().ToString() : 
                address.First().HostName.Substring(0, address.First().HostName.Length - 1);
            var port = address.First().Port;

            return $"http://{host}:{port}";
        }
    }
}
