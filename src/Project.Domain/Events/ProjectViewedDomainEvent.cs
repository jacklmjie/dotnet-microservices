using MediatR;
using Project.Domain.AggregatesModel.ProjectAggregate;

namespace Project.Domain.Events
{
    public class ProjectViewedDomainEvent : INotification
    {
        public string Name { get; set; }

        public ProjectViewer Viewer { get; set; }

        public ProjectViewedDomainEvent(string name, ProjectViewer viewer)
        {
            Name = name;
            Viewer = viewer;
        }
    }
}
