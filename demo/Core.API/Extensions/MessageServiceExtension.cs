using System;
using Microsoft.Extensions.DependencyInjection;

namespace Core.API.Extensions
{
    public static class MessageServiceExtension
    {
        public static void AddMessage(this IServiceCollection services, Action<MessageServiceBuilder> configure)
        {
//            services.AddSingleton<IMessageService, EmailService>();

            var builder = new MessageServiceBuilder(services);
            configure(builder);
        }

    }
}
