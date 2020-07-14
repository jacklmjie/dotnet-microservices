using Core.API.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Core.API.Extensions
{
    public class MessageServiceBuilder
    {
        public IServiceCollection ServiceCollection { get; set; }

        public MessageServiceBuilder(IServiceCollection services)
        {
            ServiceCollection = services;
        }

        public void UseEmail()
        {
            ServiceCollection.AddSingleton<IMessageService, MessageEmailService>();
        }

        public void UseSms()
        {
            ServiceCollection.AddSingleton<IMessageService, MessageSmsService>();
        }
    }
}
