using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Infrastructure.EntityConfigurations
{
    class ProjectEntityTypeConfiguration
        : IEntityTypeConfiguration<Project.Domain.AggregatesModel.ProjectAggregate.Project>
    {
        public void Configure(EntityTypeBuilder<Domain.AggregatesModel.ProjectAggregate.Project> builder)
        {
            builder.ToTable("Projects");
            builder.HasKey(cr => cr.Id);
        }
    }
}
