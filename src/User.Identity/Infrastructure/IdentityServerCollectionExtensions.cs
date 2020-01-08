using IdentityServer4.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using User.Identity.Authentication;

namespace User.Identity.Infrastructure
{
    /// <summary>
    /// IdentityServer 4
    /// </summary>
    public static class IdentityServerCollectionExtensions
    {
        public static IServiceCollection AddMyIdentityServer(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddTransient<IProfileService, ProfileServices>();

            services.AddIdentityServer()
               .AddExtensionGrantValidator<SmsAuthCodeValidator>()
               .AddDeveloperSigningCredential()
               .AddInMemoryClients(Config.GetClients())
               .AddInMemoryIdentityResources(Config.GetIdentityResources())
               .AddInMemoryApiResources(Config.GetApiResources());

            return services;
        }
    }
}
