using MediatR;
using Project.Domain.AggregatesModel.ProjectAggregate;

namespace Project.Domain.Events
{
    public class ProjectViewedDomainEvent : INotification
    {
        public ProjectViewer Viewer { get; set; }

        public ProjectViewedDomainEvent(ProjectViewer viewer)
        {
            Viewer = viewer;
        }
    }
}
