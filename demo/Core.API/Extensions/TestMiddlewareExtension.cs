using Core.API.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Core.API.Extensions
{
    public static class TestMiddlewareExtension
    {
        public static IApplicationBuilder UseTest(this IApplicationBuilder app)
        {
            return app.UseMiddleware<TestMiddleware>();
        }
    }
}
