using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Gateway.API
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var authenticationProviderKey = "testkey";
            services.AddAuthentication()
                .AddIdentityServerAuthentication(authenticationProviderKey, o =>
                {
                    o.Authority = "http://localhost:5001";
                    o.ApiName = "gateway_api";
                    o.SupportedTokens = SupportedTokens.Both;
                    o.ApiSecret = "secret";
                    o.RequireHttpsMetadata = false;
                });

            services.AddOcelot();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseOcelot();
        }
    }
}
