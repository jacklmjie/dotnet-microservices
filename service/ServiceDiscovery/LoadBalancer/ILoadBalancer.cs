using System.Collections.Generic;

namespace ServiceDiscovery.LoadBalancer
{
    public interface ILoadBalancer
    {
        string Resolve(IList<string> services);
    }
}
