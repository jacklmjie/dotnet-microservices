using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.API.Models;

namespace User.API.Application.EntityConfigurations
{
    class UserPropertyEntityTypeConfiguration
       : IEntityTypeConfiguration<UserProperty>
    {
        public void Configure(EntityTypeBuilder<UserProperty> builder)
        {
            builder.ToTable("UserProperties")
                .HasKey(u => new { u.Key, u.AppUserId, u.Value });
            builder.Property(x => x.Key).HasMaxLength(100);
            builder.Property(x => x.Value).HasMaxLength(100);
        }
    }
}
