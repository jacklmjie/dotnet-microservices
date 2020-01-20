using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.AggregatesModel.ProjectAggregate;

namespace Project.Infrastructure.EntityConfigurations
{
    class ProjectVisibleRuleEntityTypeConfiguration
       : IEntityTypeConfiguration<ProjectVisibleRule>
    {
        public void Configure(EntityTypeBuilder<ProjectVisibleRule> builder)
        {
            builder.ToTable("ProjectVisibleRules");
            builder.HasKey(cr => cr.Id);
        }
    }
}
