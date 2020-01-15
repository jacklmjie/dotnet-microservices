using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using User.API.Infrastructure.EntityConfigurations;
using User.API.Models;

namespace User.API.Infrastructure
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<BpFile> BpFiles { get; set; }
        public DbSet<UserProperty> UserProperties { get; set; }
        public DbSet<UserTage> UserTages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new AppUserEntityTypeConfiguration());
            builder.ApplyConfiguration(new UserPropertyEntityTypeConfiguration());
            builder.ApplyConfiguration(new UserTageEntityTypeConfiguration());
            builder.ApplyConfiguration(new BpFileEntityTypeConfiguration());
        }
    }

    //public class DataContextFactory : IDesignTimeDbContextFactory<UserContext>
    //{
    //    public UserContext CreateDbContext(string[] args)
    //    {
    //        IConfigurationRoot configuration = new ConfigurationBuilder()
    //           .SetBasePath(Directory.GetCurrentDirectory())
    //           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    //           .AddEnvironmentVariables()
    //           .Build();

    //        var connectionString = configuration["ConnectionString"];
    //        var optionsBuilder = new DbContextOptionsBuilder<UserContext>()
    //            .UseMySql(connectionString);
    //        return new UserContext(optionsBuilder.Options);
    //    }
    //}
}
