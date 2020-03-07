using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Project.API.Application.Services;
using MediatR;
using System.Reflection;
using Project.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Project.API.Application.Queries;
using Consul;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Project.Domain.AggregatesModel.ProjectAggregate;
using Project.Infrastructure.Repositories;
using DotNetCore.CAP.Dashboard.NodeDiscovery;
using Project.API.Infrastructure.Services;
using DnsClient;

namespace Project.API
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
            services.AddDbContext<ProjectContext>(options =>
            {
                options.UseMySql(configuration.GetSection("ConnectionString").Value, sql =>
                {
                    sql.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                });
            });

            services.AddScoped<IRecommendService, TestRecommendService>()
                .AddScoped<IProjectQueries, ProjectQueries>(sp =>
                {
                    return new ProjectQueries(configuration.GetSection("ConnectionString").Value);
                }).AddScoped<IProjectRepository, ProjectRepository>(sp =>
                {
                    var projectContext = sp.GetRequiredService<ProjectContext>();
                    return new ProjectRepository(projectContext);
                });

            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);

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
                options.Audience = "project_api";
                options.Authority = "http://localhost";
                options.SaveToken = true;
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

        public static IServiceCollection AddCustomCap(this IServiceCollection services)
        {
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
                    d.CurrentNodePort = 5003;
                    d.NodeId = "3";
                    d.NodeName = "CAP ProjectAPI Node";
                });
            });

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
