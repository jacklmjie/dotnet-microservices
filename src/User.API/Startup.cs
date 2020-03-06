using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Consul;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DotNetCore.CAP.Dashboard.NodeDiscovery;
using User.API.Infrastructure.Filters;
using User.API.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using User.API.Infrastructure.Services;
using System;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Linq;

namespace User.API
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
                    .AddCustomAuthentication()
                    .AddCustomCap()
                    .AddConsulServiceDiscovery(Configuration);
            services.AddControllers().AddNewtonsoftJson();
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            }).SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<UserContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<UserContextSeed>>();
                new UserContextSeed().SeedAsync(context, logger).Wait();
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
            services.AddDbContext<UserContext>(options =>
            {
                options.UseMySql(configuration.GetSection("ConnectionString").Value);
            });
            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityService, IdentityService>();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.Audience = "user_api";
                options.Authority = "http://localhost";
                options.SaveToken = true;
            });

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
                    d.CurrentNodePort = 5000;
                    d.NodeId = "1";
                    d.NodeName = "CAP No.1 Node";
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
