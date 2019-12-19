using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using User.API.Data;
using Microsoft.EntityFrameworkCore;
using User.API.Entity.Models;
using User.API.Filters;
using Core.Data.Infrastructure;
using System.Reflection;
using User.API.Data.IRepository;
using User.API.Data.Repository;
using Microsoft.Extensions.Hosting;

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
            services.AddDbContext<UserContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("MysqlUser"));
            });
            services.AddDapperDBContext<UserDapperContext>(options =>
            {
                options.Configuration = Configuration["ConnectionStrings:MysqlUser"]; ;
            });
            services.AddControllers();
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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
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
