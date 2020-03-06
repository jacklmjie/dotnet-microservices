using MediatR;
using Project.Domain.AggregatesModel.ProjectAggregate;

namespace Project.Domain.Events
{
    public class ProjectJoinedDomainEvent : INotification
    {
        public string Name { get; set; }
        public ProjectContributor Contributor { get; set; }
        public ProjectJoinedDomainEvent(string name, ProjectContributor contributor)
        {
            Name = name;
            Contributor = contributor;
        }
    }
}
