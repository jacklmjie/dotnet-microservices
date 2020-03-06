using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Recommend.API.IntegrationEvents;
using Recommend.API.Infrastructure;
using Recommend.API.Models;
using Recommend.API.Infrastructure.Services;

namespace Recommend.API.IntegrationEventsHandlers
{
    public class ProjectCreatedIntegrationEventHandler : ICapSubscribe
    {
        private RecommendDbContext _dbContext;
        private IUserService _userService;
        private IContactService _contactService;
        public ProjectCreatedIntegrationEventHandler(RecommendDbContext dbContext,
            IUserService userService,
            IContactService contactService)
        {
            _dbContext = dbContext;
            _userService = userService;
            _contactService = contactService;
        }

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
