using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.API.Models;

namespace User.API.Application.EntityConfigurations
{
    class UserTageEntityTypeConfiguration
       : IEntityTypeConfiguration<UserTage>
    {
        public void Configure(EntityTypeBuilder<UserTage> builder)
        {
            builder.ToTable("UserTages")
                .HasKey(u => new { u.AppUserId, u.Tag });
            builder.Property(x => x.Tag).HasMaxLength(100);
        }
    }
}
