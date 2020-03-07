using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Recommend.API.IntegrationEvents;
using Recommend.API.Infrastructure;
using Recommend.API.Models;
using Recommend.API.Infrastructure.Services;
using Recommend.API.IntegrationEvents.Events;

namespace Recommend.API.IntegrationEventsHandlers.EventsHandlers
{
    public class ProjectCreatedIntegrationEventHandler : ICapSubscribe
    {
        private readonly RecommendDbContext _dbContext;
        private readonly IUserService _userService;
        private readonly IContactService _contactService;
        public ProjectCreatedIntegrationEventHandler(RecommendDbContext dbContext,
            IUserService userService,
            IContactService contactService)
        {
            _dbContext = dbContext;
            _userService = userService;
            _contactService = contactService;
        }

        [CapSubscribe("project.api.project_created_event")]
        public async Task CreatedRecommendFromProject(ProjectCreatedIntegrationEvent @event)
        {
            var fromUser = await _userService.GetUserAsync(@event.UserId);
            var contacts = await _contactService.GetContactsByUserIdAsync(@event.UserId);
            foreach (var conatct in contacts)
            {
                var recommend = new ProjectRecommend
                {
                    FromUserId = @event.UserId,
                    FromUserName = fromUser.Name,
                    ProjectId = @event.ProjectId,
                    ProjectName = @event.Name,
                    CreatedTime = @event.CreatedTime,
                    RecommenTime = DateTime.Now,
                    UserId = conatct.UserId
                };
                _dbContext.ProjectRecommends.Add(recommend);
            }

            _dbContext.SaveChanges();
        }
    }
}
