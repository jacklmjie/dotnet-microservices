using System.Linq;
using System.Threading.Tasks;
using ConsulServiceRegistration;
using Core.API.Extensions;
using Core.API.Filters;
using Core.API.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Core.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc(options =>
            {
                options.Filters.Add<OperationCancelledExceptionFilter>();
            });
            // 比较规范的服务封装
            services.AddMessage(builder => builder.UseSms());
            //使用健康检查
            services.AddHealthChecks();
            services.AddConsul();

            ////添加多个检查
            //services.AddHealthChecks()
            //    .AddCheck<SqlServerHealthCheck>("sql_check")
            //    .AddCheck<RedisHealthCheck>("redis_check");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<ConsulServiceOptions> serviceOptions)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 配置健康检测地址，.NET Core 内置的健康检测地址中间件
            app.UseHealthChecks(serviceOptions.Value.HealthCheck);
            app.UseConsul();

            app.UseRouting();

            app.UseAuthorization();

            // 通用的添加中间件的方法
            app.UseMiddleware<TestMiddleware>();
            app.UseTest();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //    //使用该扩展方法
            //    endpoints.MapHealthChecks("/health");

            //    //单独检查
            //    endpoints.MapHealthChecks("/redishealth", new HealthCheckOptions()
            //    {
            //        Predicate = s => s.Name.Equals("redis_check"),
            //        ResponseWriter = WriteResponse
            //    });
            //});
        }

        //指定健康检查返回格式
        private static Task WriteResponse(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json";

            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(pair =>
                    new JProperty(pair.Key, new JObject(
                        new JProperty("status", pair.Value.Status.ToString()),
                        new JProperty("description", pair.Value.Description),
                        new JProperty("data", new JObject(pair.Value.Data.Select(
                            p => new JProperty(p.Key, p.Value))))))))));

            return context.Response.WriteAsync(json.ToString());
        }
    }
}
