using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using User.API.Application.Middlewares;

namespace User.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                 .UseFailing(options => {
                     options.ConfigPath = "/Failing";
                     options.NotFilteredPaths.AddRange(new[] { "/hc", "/liveness" });
                 })
                .UseStartup<Startup>();
    }
}
