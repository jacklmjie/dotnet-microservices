using System.Net;

namespace Contact.API
{
    public class ContactDBContextSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ContactBooksCollectionName { get; set; }
        public string ContactApplyRequestsCollectionName { get; set; }
    }

    public class ServiceDiscoveryOptions
    {
        public string ServiceName { get; set; }
        public string UserServiceName { get; set; }
        public ConsulOptions Consul { get; set; }
    }

    public class ConsulOptions
    {
        public string HttpEndpoint { get; set; }

        public DnsEndpoint DnsEndpoint { get; set; }
    }

    public class DnsEndpoint
    {
        public string Address { get; set; }

        public int Port { get; set; }

        public IPEndPoint ToIPEndPoint()
        {
            return new IPEndPoint(IPAddress.Parse(Address), Port);
        }
    }
}
