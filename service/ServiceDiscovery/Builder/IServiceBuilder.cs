using System;
using System.Threading.Tasks;
using ServiceDiscovery.LoadBalancer;

namespace ServiceDiscovery
{
    public interface IServiceBuilder
    { 
        // 服务提供者！
        IServiceProvider ServiceProvider { get; set; }

        // 服务名称!
        string ServiceName { get; set; }

        // Uri方案
        string UriScheme { get; set; }

        // 你用哪种策略？
        ILoadBalancer LoadBalancer  { get; set; }

        Task<Uri> BuildAsync(string path);
    }
}
