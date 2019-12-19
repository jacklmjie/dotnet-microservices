using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using User.Identity.Services;
using System.Net.Http;
using System;
using System.Net.Http.Headers;
using Microsoft.Extensions.Hosting;

namespace User.Identity
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer()
                .AddExtensionGrantValidator<Authentication.SmsAuthCodeValidator>()
                .AddDeveloperSigningCredential()
                .AddInMemoryClients(Config.GetClients())
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources());

            services.AddScoped<IAuthCodeService, TestAuthCodeService>()
                .AddScoped<IUserService, UserService>();
            services.AddHttpClient("user_api", c =>
            {
                c.BaseAddress = new Uri("http://localhost:5000");
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseIdentityServer();
        }
    }
}
