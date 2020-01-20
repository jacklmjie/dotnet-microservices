using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.AggregatesModel.ProjectAggregate;

namespace Project.Infrastructure.EntityConfigurations
{
    class ProjectPropertyEntityTypeConfiguration
       : IEntityTypeConfiguration<ProjectProperty>
    {
        public void Configure(EntityTypeBuilder<ProjectProperty> builder)
        {
            builder.ToTable("ProjectProperties");
            builder.HasKey(cr => new { cr.ProjectId, cr.Key, cr.Value });

            builder.Property(cr => cr.Key).HasMaxLength(100);
            builder.Property(cr => cr.Value).HasMaxLength(100);
        }
    }
}
