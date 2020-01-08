using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using User.API.Data;
using User.API.Models;

namespace User.API.Infrastructure
{
    /// <summary>
    /// 初始化数据
    /// </summary>
    public static class UserContextServiceCollectionExtensions
    {
        public static IApplicationBuilder UserInitDatabase(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var userContext = scope.ServiceProvider.GetRequiredService<UserContext>();
                userContext.Database.Migrate();

                if (!userContext.Users.Any())
                {
                    userContext.Users.Add(new AppUser() { Name = "jack.li" });
                    userContext.SaveChanges();
                }
            }

            return app;
        }
    }
}
