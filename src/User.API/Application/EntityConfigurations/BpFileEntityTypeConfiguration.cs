using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.API.Models;

namespace User.API.Application.EntityConfigurations
{
    class BpFileEntityTypeConfiguration
       : IEntityTypeConfiguration<BpFile>
    {
        public void Configure(EntityTypeBuilder<BpFile> builder)
        {
            builder.ToTable("BpFiles")
              .HasKey(u => u.Id);
        }
    }
}
