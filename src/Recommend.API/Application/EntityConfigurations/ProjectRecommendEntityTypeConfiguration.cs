using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recommend.API.Models;

namespace Recommend.API.Application.EntityConfigurations
{
    class ProjectRecommendEntityTypeConfiguration
       : IEntityTypeConfiguration<ProjectRecommend>
    {
        public void Configure(EntityTypeBuilder<ProjectRecommend> builder)
        {
            builder.ToTable("ProjectRecommends")
            .HasKey(u => u.Id);
        }
    }
}
