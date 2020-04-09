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
using User.API.Application.Filters;
using User.API.Application;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using User.API.Services;
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
                    .AddCustomCap(Configuration)
                    .AddConsulServiceDiscovery(Configuration);
            services.AddControllers().AddNewtonsoftJson();

            //添加健康检查
            services.AddHealthChecks();

            //异常过滤器
            //MVC中间件之前的一些错误，其实是捕获不到的,仅仅关心控制器之间的异常
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
            else
            {
                //默认异常中间件ExceptionHandlerMiddleware,有参数ExceptionHandlingPath/ExceptionHandler
                //和放置位置有关，第一个所有错误都能捕捉到
                app.UseExceptionHandler("/Error");

                //自定义异常中间件
                //app.UseMiddleware<MyExceptionMiddleware>();
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
                //健康检查地址
                endpoints.MapHealthChecks("/health");
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
                options.UseMySql(configuration.GetConnectionString("MySqlUser"));
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

        public static IServiceCollection AddCustomCap(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddCap(x =>
            {
                x.UseMySql(configuration.GetConnectionString("CapDashboard"));
                x.UseRabbitMQ("localhost");
                x.UseDashboard();
                x.UseDiscovery(d =>
                {
                    d.DiscoveryServerHostName = "localhost";
                    d.DiscoveryServerPort = 8500;
                    d.CurrentNodeHostName = "localhost";
                    d.CurrentNodePort = 5000;
                    d.NodeId = "1";
                    d.NodeName = "CAP UserAPI Node";
                });
            });

            return services;
        }

        public static IServiceCollection AddConsulServiceDiscovery(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection("ServiceDiscovery").Get<ServiceDiscoveryOptions>();
            services.Configure<ServiceDiscoveryOptions>(configuration.GetSection("ServiceDiscovery"));
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
                    HTTP = new Uri(address, "health").OriginalString
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
