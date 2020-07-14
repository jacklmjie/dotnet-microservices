using System;

namespace ServiceDiscovery
{
    public static class  ServiceProviderExtension
    {
        public static IServiceBuilder CreateServiceBuilder(this IServiceProvider serviceProvider, Action<IServiceBuilder> config)
        {
            
            var builder = new ServiceBuilder(serviceProvider);
            config(builder);
            return builder;
        } 
    }
}
