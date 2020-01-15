using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Linq;
using System.Threading.Tasks;
using User.API.Models;

namespace User.API.Infrastructure
{
    public class UserContextSeed
    {
        public async Task SeedAsync(UserContext context, ILogger<UserContextSeed> logger, int retries = 3)
        {
            var policy = CreatePolicy(retries, logger, nameof(UserContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                if (!context.Users.Any())
                {
                    await context.Users.AddAsync(new AppUser() { Name = "jack.li" });
                    await context.SaveChangesAsync();
                }
            });
        }

        private AsyncRetryPolicy CreatePolicy(int retries, ILogger<UserContextSeed> logger, string prefix)
        {
            return Policy.Handle<Exception>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }
}
