using DotNetCore.CAP;
using MediatR;
using Project.API.Application.IntegrationEvents;
using Project.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Project.API.Application.DomainEventHandlers
{
    public class ProjectViewedDomainEventHandler : INotificationHandler<ProjectViewedDomainEvent>
    {
        private readonly ICapPublisher _capPublisher;
        public ProjectViewedDomainEventHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public Task Handle(ProjectViewedDomainEvent notification, CancellationToken cancellationToken)
        {
            var @event = new ProjectViewedIntegrationEvent
            {
                Name = notification.Name,
                Viewer = notification.Viewer
            };

            _capPublisher.Publish("project.api.project_viewed_event", @event);

            return Task.CompletedTask;
        }
    }
}
