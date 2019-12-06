using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using User.API.Entity.Models;

namespace User.API.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>()
                .ToTable("Users")
                .HasKey(u => u.Id);

            modelBuilder.Entity<UserProperty>().Property(x => x.Key).HasMaxLength(100);
            modelBuilder.Entity<UserProperty>().Property(x => x.Value).HasMaxLength(100);
            modelBuilder.Entity<UserProperty>()
                .ToTable("UserProperties")
                .HasKey(u => new { u.Key, u.AppUserId, u.Value });

            modelBuilder.Entity<UserTage>().Property(x => x.Tag).HasMaxLength(100);
            modelBuilder.Entity<UserTage>()
                .ToTable("UserTages")
                .HasKey(u => new { u.AppUserId, u.Tag });

            modelBuilder.Entity<BpFile>()
               .ToTable("BpFiles")
               .HasKey(u => u.Id);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<BpFile> BpFiles { get; set; }
        public DbSet<UserProperty> UserProperties { get; set; }
        public DbSet<UserTage> UserTages { get; set; }
    }

    ///// <summary>
    ///// 为EF的Migration准备的辅助类，填写完整连接字符串和数据库类型
    ///// 就可以使用Add-Migration和Update-Database了
    ///// </summary>
    //public class DataContextFactory : IDesignTimeDbContextFactory<UserContext>
    //{
    //    public UserContext CreateDbContext(string[] args)
    //    {
    //        var builder = new DbContextOptionsBuilder<UserContext>();
    //        builder.UseMySql("server=127.0.0.1;port=3306;database=user_api;uid=root;pwd=password;IgnoreCommandTransaction=true;");
    //        return new UserContext(builder.Options);
    //    }
    //}
}
