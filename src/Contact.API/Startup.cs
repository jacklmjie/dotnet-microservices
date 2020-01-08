using System.Net.Http;
using Contact.API.Data;
using Contact.API.Infrastructure;
using Contact.API.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            services.AddOptions();
            services.AddMyContactDBContext<ContactDBContext>(options =>
            {
                options = Configuration.GetSection("ContactDBContextSettings").Get<ContactDBContextSettings>();
            });
            services.AddMyServiceDiscovery(options =>
            {
                options = Configuration.GetSection("ServiceDiscovery").Get<ServiceDiscoveryOptions>();
            });
            services.AddMyAuthentication();

            services.AddSingleton(new HttpClient());
            services.AddScoped<IUserService, UserService>();

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
}
