using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DotNetCore.CAP.Dashboard.NodeDiscovery;
using User.API.Application.Filters;
using User.API.Application;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using User.API.Services;
using System;
using User.API.Dtos;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using User.API.Application.Extension;

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
                    .AddCustomSwagger(Configuration);
            services.AddControllers().AddNewtonsoftJson();

            services.AddHealthChecks();

            //添加redis
            //services.AddSingleton<ConnectionMultiplexer>(sp =>
            //{
            //    var redisConnectionString = Configuration.GetConnectionString("Redis");
            //    var configuration = ConfigurationOptions.Parse(redisConnectionString, true);

            //    configuration.ResolveDns = true;

            //    return ConnectionMultiplexer.Connect(configuration);
            //});
            //services.AddTransient<IUserRepository, RedisUserRepository>();

            //异常过滤器
            //MVC中间件之前的一些错误，其实是捕获不到的,仅仅关心控制器之间的异常
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                options.Filters.Add(typeof(ValidateModelStateFilter));
            }).SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Error");
            }

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<UserContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<UserContextSeed>>();
                new UserContextSeed().SeedAsync(context, logger).Wait();
            }

            app.UseRouting();

            app.UseAuthorization();

            string pathBase = "/api";
            app.UsePathBase(pathBase);
            app.UseSwagger()
             .UseSwaggerUI(c =>
             {
                 c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "User.API V1");
                 c.OAuthClientId("userswaggerui");
                 c.OAuthAppName("user Swagger UI");
             });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            //服务注册
            app.RegisterConsul(Configuration, lifetime);
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

            //add custom application services
            services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();

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

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "User HTTP API",
                    Version = "v1",
                    Description = "The User Service HTTP API"
                });
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {
                        Implicit = new OpenApiOAuthFlow()
                        {
                            AuthorizationUrl = new Uri($"http://localhost/connect/authorize"),
                            TokenUrl = new Uri($"http://localhost/connect/token"),
                            Scopes = new Dictionary<string, string>()
                            {
                                { "user", "user_api" }
                            }
                        }
                    }
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            return services;
        }
    }
}
