using DotNetCore.CAP;
using MediatR;
using Project.API.Application.IntegrationEvents;
using Project.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Project.API.Application.DomainEventHandlers
{
    public class ProjectJoinedDomainEventHandler : INotificationHandler<ProjectJoinedDomainEvent>
    {
        private readonly ICapPublisher _capPublisher;
        public ProjectJoinedDomainEventHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public Task Handle(ProjectJoinedDomainEvent notification, CancellationToken cancellationToken)
        {
            var @event = new ProjectJoinedIntegrationEvent
            {
                Name = notification.Name,
                Contributor = notification.Contributor
            };

            _capPublisher.Publish("project.api.project_joined_event", @event);

            return Task.CompletedTask;
        }
    }
}
