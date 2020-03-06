using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using MediatR;
using Project.API.Application.IntegrationEvents;
using Project.Domain.Events;

namespace Project.API.Application.DomainEventHandlers
{
    public class ProjectCreatedDomainEventHandler : INotificationHandler<ProjectCreatedDomainEvent>
    {
        private readonly ICapPublisher _capPublisher;
        public ProjectCreatedDomainEventHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public Task Handle(ProjectCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var @event = new ProjectCreatedIntegrationEvent
            {
                ProjectId = notification.Project.Id,
                UserId = notification.Project.UserId,
                Name = notification.Project.Name,
                CreatedTime = DateTime.Now
            };

            _capPublisher.Publish("project.api.project_created_event", @event);

            return Task.CompletedTask;
        }
    }
}
