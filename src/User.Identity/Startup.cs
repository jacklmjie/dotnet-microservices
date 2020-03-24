using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using User.Identity.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using IdentityServer4.Services;
using User.Identity.Authentication;
using User.Identity.Services;
using System.Net.Http;
using DnsClient;
using Polly;
using Polly.Extensions.Http;
using System;

namespace User.Identity
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
            services.AddCustomIntegrations()
                 .AddCustomIdentityServer()
                 .AddConsulServiceDiscovery(Configuration);
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseIdentityServer();
        }
    }

    static class CustomExtensionsMethods
    {
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            //重试3次，可以加熔断
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (msg, re) =>
                {
                    //log
                    Console.WriteLine(msg.Result);
                    Console.WriteLine(re.TotalSeconds);
                });
        }

        public static IServiceCollection AddCustomIntegrations(this IServiceCollection services)
        {
            services.AddOptions();
            services.AddScoped<IAuthCodeService, TestAuthCodeService>()
                .AddScoped<IUserService, UserService>();

            services.AddHttpClient<IUserService, UserService>()
              .AddPolicyHandler(GetRetryPolicy());
            return services;
        }

        public static IServiceCollection AddCustomIdentityServer(this IServiceCollection services)
        {
            services.AddTransient<IProfileService, ProfileServices>();
            services.AddIdentityServer()
               .AddExtensionGrantValidator<SmsAuthCodeValidator>()
               .AddDeveloperSigningCredential()
               .AddInMemoryClients(Config.GetClients())
               .AddInMemoryIdentityResources(Config.GetIdentityResources())
               .AddInMemoryApiResources(Config.GetApiResources());

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
