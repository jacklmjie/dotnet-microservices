using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;
using User.Identity.Services;

namespace User.Identity.Infrastructure
{
    /// <summary>
    /// Polly
    /// </summary>
    public static class PolicyServerCollectionExtensions
    {
        public static IServiceCollection AddMyPolicy(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddHttpClient<IUserService, UserService>()
               .AddPolicyHandler(GetRetryPolicy());

            return services;
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            //重试3次，可以加熔断
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (msg, re) =>
                {
                    //log
                    Console.WriteLine(msg.Result);
                    Console.WriteLine(re.TotalSeconds);
                });
        }
    }
}
