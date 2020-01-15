using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.API.Models;

namespace User.API.Infrastructure.EntityConfigurations
{
    class AppUserEntityTypeConfiguration
       : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.ToTable("Users")
            .HasKey(u => u.Id);
        }
    }
}
