using Project.Domain.AggregatesModel.ProjectAggregate;

namespace Project.API.Application.IntegrationEvents
{
    public class ProjectViewedIntegrationEvent
    {
        public string Name { get; set; }

        public ProjectViewer Viewer { get; set; }
    }
}
