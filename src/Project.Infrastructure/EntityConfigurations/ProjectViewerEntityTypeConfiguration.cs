using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.AggregatesModel.ProjectAggregate;

namespace Project.Infrastructure.EntityConfigurations
{
    class ProjectViewerEntityTypeConfiguration
       : IEntityTypeConfiguration<ProjectViewer>
    {
        public void Configure(EntityTypeBuilder<ProjectViewer> builder)
        {
            builder.ToTable("ProjectViewers");
            builder.HasKey(cr => cr.Id);
        }
    }
}
