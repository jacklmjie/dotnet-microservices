using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Consul;
using DnsClient;
using DotNetCore.CAP.Dashboard.NodeDiscovery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Recommend.API.Infrastructure;
using Recommend.API.Infrastructure.Services;
using Recommend.API.IntegrationEventsHandlers.EventsHandlers;

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

            app.UseConsulHealthChecks(Configuration);

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

            services.AddSingleton(new HttpClient());
            services.AddScoped<IUserService, UserService>()
                    .AddScoped<IContactService, ContactService>();
            return services;
        }

        public static IServiceCollection AddCustomCap(this IServiceCollection services)
        {
            services.AddScoped<ProjectCreatedIntegrationEventHandler>();
            services.AddCap(x =>
            {
                x.UseMySql("server=127.0.0.1;port=3306;database=project_coantact_cap;uid=root;pwd=password;");
                x.UseRabbitMQ("localhost");
                x.UseDashboard();
                x.UseDiscovery(d =>
                {
                    d.DiscoveryServerHostName = "localhost";
                    d.DiscoveryServerPort = 8500;
                    d.CurrentNodeHostName = "localhost";
                    d.CurrentNodePort = 5004;
                    d.NodeId = "4";
                    d.NodeName = "CAP RecommendAPI Node";
                });
            });

            return services;
        }

        public static IServiceCollection AddConsulServiceDiscovery(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection("ServiceDiscovery").Get<ServiceDiscoveryOptions>();
            services.AddSingleton<IConsulClient>(p => new ConsulClient(cfg =>
            {
                if (!string.IsNullOrEmpty(options.Consul.HttpEndpoint))
                {
                    cfg.Address = new Uri(options.Consul.HttpEndpoint);
                }
            }));

            //services.Configure<ServiceDiscoveryOptions>(configuration.GetSection("ServiceDiscovery"));
            services.AddSingleton<IDnsQuery>(p =>
            {
                return new LookupClient(options.Consul.DnsEndpoint.ToIPEndPoint());
            });
            services.AddSingleton<IServiceDiscovery, ServiceDiscovery>();

            return services;
        }

        public static IApplicationBuilder UseConsulHealthChecks(this IApplicationBuilder app, IConfiguration configuration)
        {
            var options = configuration.GetSection("ServiceDiscovery").Get<ServiceDiscoveryOptions>();
            var appLife = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>() ??
               throw new ArgumentException("Missing Dependency", nameof(IHostApplicationLifetime));

            var consul = app.ApplicationServices.GetRequiredService<IConsulClient>() ??
               throw new ArgumentException("Missing Dependency", nameof(IConsulClient));

            if (string.IsNullOrEmpty(options.ServiceName))
                throw new ArgumentException("service name must be configure", nameof(options.ServiceName));

            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>()
                .Addresses
                .Select(p => new Uri(p));

            foreach (var address in addresses)
            {
                var serviceId = $"{options.ServiceName}_{address.Host}:{address.Port}";

                var httpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                    Interval = TimeSpan.FromSeconds(30),
                    HTTP = new Uri(address, "HealthCheck").OriginalString
                };

                var registration = new AgentServiceRegistration()
                {
                    Checks = new[] { httpCheck },
                    Address = address.Host,
                    ID = serviceId,
                    Name = options.ServiceName,
                    Port = address.Port,
                    Tags = new[] { "api" }
                };

                consul.Agent.ServiceRegister(registration).GetAwaiter().GetResult();

                appLife.ApplicationStopping.Register(() =>
                {
                    consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
                });
            }

            return app;
        }
    }
}
