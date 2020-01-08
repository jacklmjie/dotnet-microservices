using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace User.API.Infrastructure
{
    /// <summary>
    /// JWT权限认证
    /// </summary>
    public static class AuthenticationServiceCollectionExtensions
    {
        public static IServiceCollection AddMyAuthentication(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Audience = "user_api";
                    options.Authority = "http://localhost";
                    options.SaveToken = true;
                });

            return services;
        }
    }
}
