using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using Contact.API.Infrastructure;
using Contact.API.Infrastructure.Event;
using Contact.API.Infrastructure.Repositories;
using Contact.API.Infrastructure.Services;
using DnsClient;
using DotNetCore.CAP.Dashboard.NodeDiscovery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Contact.API
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
        public static IServiceCollection AddCustomIntegrations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<ContactDBContextSettings>(configuration.GetSection("ContactDBContextSettings"));
            services.AddScoped<ContactDBContext>()
                .AddScoped<IContactApplyRequestRepository, MongoContactApplyRequestRepository>()
                .AddScoped<IContactRepository, MongoContactRepository>();
            services.AddSingleton(new HttpClient());
            services.AddScoped<IUserService, UserService>();
            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityService, IdentityService>();

            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.Audience = "contact_api";
                options.Authority = "http://localhost";
                options.SaveToken = true;
            });

            return services;
        }

        public static IServiceCollection AddCustomCap(this IServiceCollection services)
        {
            services.AddTransient<ISubscriberService, SubscriberService>();
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
                    d.CurrentNodePort = 5002;
                    d.NodeId = "2";
                    d.NodeName = "CAP No.2 Node";
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
