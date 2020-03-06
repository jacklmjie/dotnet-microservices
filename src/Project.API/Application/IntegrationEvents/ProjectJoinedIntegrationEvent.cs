using Project.Domain.AggregatesModel.ProjectAggregate;

namespace Project.API.Application.IntegrationEvents
{
    public class ProjectJoinedIntegrationEvent
    {
        public string Name { get; set; }
        public ProjectContributor Contributor { get; set; }
    }
}
