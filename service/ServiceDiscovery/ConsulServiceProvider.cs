using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Consul;

namespace ServiceDiscovery
{
    public class ConsulServiceProvider : IServiceProvider
    {
        private readonly ConsulClient _consuleClient;

        public ConsulServiceProvider(Uri uri)
        {
            _consuleClient = new ConsulClient(consulConfig =>
            {
                consulConfig.Address = uri;
            });
        }

        public async Task<IList<string>> GetServicesAsync(string serviceName)
        {
            // Health 当前consul里已注册的服务，健康检查的信息也拿过来
            // HTTP API 8500端口
            var queryResult = await _consuleClient.Health.Service(serviceName,"", true);
            var result = new List<string>();
            foreach (var service in queryResult.Response)
            {
                result.Add(service.Service.Address + ":" + service.Service.Port);
            }
            return result;
        }
    }
}
