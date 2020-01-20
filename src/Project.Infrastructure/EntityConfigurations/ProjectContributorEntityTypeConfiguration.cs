using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.AggregatesModel.ProjectAggregate;

namespace Project.Infrastructure.EntityConfigurations
{
    class ProjectContributorEntityTypeConfiguration
       : IEntityTypeConfiguration<ProjectContributor>
    {
        public void Configure(EntityTypeBuilder<ProjectContributor> builder)
        {
            builder.ToTable("ProjectContributors");
            builder.HasKey(cr => cr.Id);
        }
    }
}
