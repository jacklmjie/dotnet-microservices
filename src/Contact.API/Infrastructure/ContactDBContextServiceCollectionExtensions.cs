using Contact.API.Data;
using Microsoft.Extensions.Configuration;
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
            IConfigurationSection section)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            services.Configure<ContactDBContextSettings>(section);
            services.AddScoped<IContactApplyRequestRepository, MongoContactApplyRequestRepository>()
                 .AddScoped<IContactRepository, MongoContactRepository>()
                 .AddScoped<ContactDBContext>();

            return services;
        }
    }
}
