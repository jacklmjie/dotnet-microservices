using System.Linq;
using System.Threading.Tasks;
using Core.API.Extensions;
using Core.API.Filters;
using Core.API.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
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
            // �ȽϹ淶�ķ����װ
            services.AddMessage(builder => builder.UseSms());
            //ʹ�ý������
            services.AddHealthChecks();

            ////��Ӷ�����
            //services.AddHealthChecks()
            //    .AddCheck<SqlServerHealthCheck>("sql_check")
            //    .AddCheck<RedisHealthCheck>("redis_check");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            // ͨ�õ�����м���ķ���
            app.UseMiddleware<TestMiddleware>();
            app.UseTest();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //ʹ�ø���չ����
                endpoints.MapHealthChecks("/health");

                //�������
                endpoints.MapHealthChecks("/redishealth", new HealthCheckOptions()
                {
                    Predicate = s => s.Name.Equals("redis_check"),
                    ResponseWriter = WriteResponse
                });
            });
        }

        //ָ��������鷵�ظ�ʽ
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
