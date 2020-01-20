using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Project.Domain.Events
{
    public class ProjectCreatedDomainEvent : INotification
    {
        public AggregatesModel.ProjectAggregate.Project Project { get; set; }
        public ProjectCreatedDomainEvent(AggregatesModel.ProjectAggregate.Project project)
        {
            Project = project;
        }
    }
}
