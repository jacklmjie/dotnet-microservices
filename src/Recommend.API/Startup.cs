using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnsClient;
using DotNetCore.CAP.Dashboard.NodeDiscovery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Recommend.API.Infrastructure;
using Recommend.API.Infrastructure.Services;

namespace Recommend.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomIntegrations(Configuration)
                .AddCustomCap()
                .AddConsulServiceDiscovery(Configuration);
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    static class CustomExtensionsMethods
    {
        public static IServiceCollection AddCustomIntegrations(this IServiceCollection services,
           IConfiguration configuration)
        {
            services.AddOptions();
            services.AddDbContext<RecommendDbContext>(options =>
            {
                options.UseMySql(configuration["ConnectionString:MySqlRecommends"].ToString());
            });

            services.AddScoped<IUserService, UserService>()
                    .AddScoped<IContactService, ContactService>();
            return services;
        }

        public static IServiceCollection AddCustomCap(this IServiceCollection services)
        {
            services.AddCap(x =>
            {
                x.UseMySql("server=127.0.0.1;port=3306;database=user_cap;uid=root;pwd=password;");
                x.UseRabbitMQ("localhost");
                x.UseDashboard();
                x.UseDiscovery(d =>
                {
                    d.DiscoveryServerHostName = "localhost";
                    d.DiscoveryServerPort = 8500;
                    d.CurrentNodeHostName = "localhost";
                    d.CurrentNodePort = 5004;
                    d.NodeId = "4";
                    d.NodeName = "CAP No.4 Node";
                });
            });

            return services;
        }

        public static IServiceCollection AddConsulServiceDiscovery(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ServiceDiscoveryOptions>(configuration.GetSection("ServiceDiscovery"));
            var options = configuration.GetSection("ServiceDiscovery").Get<ServiceDiscoveryOptions>();
            services.AddSingleton<IDnsQuery>(p =>
            {
                return new LookupClient(options.Consul.DnsEndpoint.ToIPEndPoint());
            });
            services.AddSingleton<IServiceDiscovery, ServiceDiscovery>();

            return services;
        }
    }
}
