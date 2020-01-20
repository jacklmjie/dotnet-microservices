using MediatR;
using Project.Domain.AggregatesModel.ProjectAggregate;

namespace Project.Domain.Events
{
    public class ProjectJoinedDomainEvent : INotification
    {
        public ProjectContributor Contributor { get; set; }
        public ProjectJoinedDomainEvent(ProjectContributor contributor)
        {
            Contributor = contributor;
        }
    }
}
