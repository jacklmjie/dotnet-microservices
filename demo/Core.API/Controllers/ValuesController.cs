using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServiceDiscovery;
using ServiceDiscovery.LoadBalancer;

namespace Core.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public void Get()
        {
            //TODO:服务发现可以注册到HttpClient
            var serviceProvider = new ConsulServiceProvider(new Uri("http://127.0.0.1:8500"));
            var myServiceA = serviceProvider.CreateServiceBuilder(builder =>
            {
                builder.ServiceName = "MyServiceA";
                // 指定负载均衡器
                builder.LoadBalancer = TypeLoadBalancer.RandomLoad;
                // 指定Uri方案
                builder.UriScheme = Uri.UriSchemeHttp;
            });

            var httpClient = new HttpClient();
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine($"-------------第{i}次请求-------------");
                try
                {
                    var uri = myServiceA.BuildAsync("/health").Result;
                    Console.WriteLine($"{DateTime.Now} - 正在调用：{uri}");
                    var content = httpClient.GetStringAsync(uri).Result;
                    Console.WriteLine($"调用结果：{content}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"调用异常：{e.GetType()}");
                }
                Task.Delay(1000).Wait();
            }
        }

        public void GetByPolly()
        {
            var serviceProvider = new ConsulServiceProvider(new Uri("http://127.0.0.1:8500"));
            var myServiceA = serviceProvider.CreateServiceBuilder(builder =>
            {
                builder.ServiceName = "MyServiceA";
                // 指定负载均衡器
                builder.LoadBalancer = TypeLoadBalancer.RoundRobin;
                // 指定Uri方案
                builder.UriScheme = Uri.UriSchemeHttp;
            });

            var httpClient = new HttpClient();
            var policy = PolicyBuilder.CreatePolly();

            // 重试轮询+熔断降级

            // 你去一个商场消费，某个商品卖断货，你可能寻求次级品，（提供的服务，就降级了）
            // 服务A访问服务B，这个B老是失败，你失败多了，如果是高并发就会引发雪崩，我降级服务，我自己准一套备用的。

            // 返回一个个空数据

            // 源码肯定有，
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine($"-------------第{i}次请求-------------");
                policy.Execute(() =>
                {
                    try
                    {
                        // 根据负载均衡获取具体服务地址
                        // consul崩了，仍然会被我们的polly接住
                        // 服务和服务间调用也一样
                        // 服务间传消息？直接调用API,MQ
                        // 每个有服务调用的服务应用，都用POLLY，服务挂了
                        var uri = myServiceA.BuildAsync("/api/order").Result;
                        Console.WriteLine($"{DateTime.Now} - 正在调用：{uri}");
                        var content = httpClient.GetStringAsync(uri).Result;
                        Console.WriteLine($"调用结果：{content}");
                    }
                    catch (Exception e)
                    {
                        // 如果你要在策略里捕捉异常，如果这个异常还是你定义的故障，一定要把这异常再抛出来
                        Console.WriteLine($"调用异常：{e.GetType()}");
                        throw;
                    }

                });
                Task.Delay(1000).Wait();
            }
        }
    }
}
