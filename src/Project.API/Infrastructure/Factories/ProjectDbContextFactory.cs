using System.IO;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.Infrastructure;

namespace Project.API.Infrastructure.Factories
{
    public class ProjectDbContextFactory : IDesignTimeDbContextFactory<ProjectContext>
    {
        public ProjectContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ProjectContext>();

            optionsBuilder.UseMySql(config["ConnectionString"], 
                o => o.MigrationsAssembly("Project.API"));

            return new ProjectContext(optionsBuilder.Options);
        }
    }
}