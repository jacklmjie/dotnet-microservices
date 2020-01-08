using Contact.API.Data;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Contact.API.Infrastructure
{
    /// <summary>
    /// MongoDB
    /// </summary>
    public static class ContactDBContextServiceCollectionExtensions
    {
        public static IServiceCollection AddMyContactDBContext<T>(this IServiceCollection services,
            Action<ContactDBContextSettings> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.Configure(setupAction);
            services.AddScoped<IContactApplyRequestRepository, MongoContactApplyRequestRepository>()
                 .AddScoped<IContactRepository, MongoContactRepository>()
                 .AddScoped<ContactDBContext>();

            return services;
        }
    }
}
