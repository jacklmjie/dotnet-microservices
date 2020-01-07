using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using User.API.Data;
using Microsoft.EntityFrameworkCore;
using User.API.Models;
using User.API.Filters;
using Core.Data.Infrastructure;
using System.Reflection;
using User.API.Data.IRepository;
using User.API.Data.Repository;
using Microsoft.Extensions.Hosting;
using User.API.Dtos;
using Consul;
using Microsoft.Extensions.Options;
using System;
using Microsoft.AspNetCore.Http.Features;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server.Features;

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
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Audience = "user_api";
                    options.Authority = "http://localhost";
                });

            services.AddDbContext<UserContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("MysqlUser"));
            });
            services.AddDapperDBContext<UserDapperContext>(options =>
            {
                options.Configuration = Configuration["ConnectionStrings:MysqlUser"];
            });
            services.AddControllers();
            services.AddOptions();
            services.Configure<ServiceDiscoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
            services.AddSingleton<IConsulClient>(p => new ConsulClient(cfg =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<ServiceDiscoveryOptions>>().Value;

                if (!string.IsNullOrEmpty(serviceConfiguration.Consul.HttpEndpoint))
                {
                    // if not configured, the client will use the default value "127.0.0.1:8500"
                    cfg.Address = new Uri(serviceConfiguration.Consul.HttpEndpoint);
                }
            }));
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(GlobalExceptionFilter));
            }).SetCompatibilityVersion(CompatibilityVersion.Latest);
            RegisterRepository(services);
        }

        private void RegisterRepository(IServiceCollection services)
        {
            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IUserPropertyRepository, UserPropertyRepository>();

            var assembly = Assembly.Load("User.API");
            var allTypes = assembly.GetTypes().Where(t =>
            t.GetTypeInfo().IsClass &&
            !t.GetTypeInfo().IsAbstract &&
            t.GetTypeInfo().Name.EndsWith("Repository"));
            foreach (var type in allTypes)
            {
                var types = type.GetInterfaces();
                foreach (var p in types)
                {
                    services.AddScoped(p, type);
                }
            }
        }

        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            IHostApplicationLifetime appLife,
            IOptions<ServiceDiscoveryOptions> serviceOptions,
            IConsulClient consul)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>()
                .Addresses
                .Select(p => new Uri(p));

            foreach (var address in addresses)
            {
                var serviceId = $"{serviceOptions.Value.ServiceName}_{address.Host}:{address.Port}";

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
                    Name = serviceOptions.Value.ServiceName,
                    Port = address.Port
                };

                consul.Agent.ServiceRegister(registration).GetAwaiter().GetResult();

                appLife.ApplicationStopping.Register(() =>
                {
                    consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
                });
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            InitUserDatabase(app);
        }

        /// <summary>
        /// 初始化数据库数据
        /// </summary>
        /// <param name="app"></param>
        public void InitUserDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var userContext = scope.ServiceProvider.GetRequiredService<UserContext>();
                userContext.Database.Migrate();

                if (!userContext.Users.Any())
                {
                    userContext.Users.Add(new AppUser() { Name = "jack.li" });
                    userContext.SaveChanges();
                }
            }
        }
    }
}
