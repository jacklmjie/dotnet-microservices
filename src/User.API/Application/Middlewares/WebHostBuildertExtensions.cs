using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace User.API.Application.Middlewares
{
    //手动启用断路器
    //中断中间件：FailingMiddleware，通过访问http://localhost:5000/failing获取该中间件的启用状态，
    //通过请求参数指定：即通过http://localhost:5000/failing?enable和http://localhost:5000/failing?disable
    //来手动中断和恢复服务，来模拟断路，以便用于测试断路器模式
    public static class WebHostBuildertExtensions
    {
        public static IWebHostBuilder UseFailing(this IWebHostBuilder builder, Action<FailingOptions> options)
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IStartupFilter>(new FailingStartupFilter(options));
            });
            return builder;
        }
    }
}
