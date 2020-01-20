using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Project.API.Application.Commands;
using MediatR;
using Project.API.Application.Services;

namespace Project.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;

        public ProjectController(IMediator mediator,
            IIdentityService identityService)
        {
            _mediator = mediator;
            _identityService = identityService;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreateProject([FromBody] Domain.AggregatesModel.ProjectAggregate.Project project)
        {
            var command = new CreateProjectCommand() { Project = project };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        [Route("view")]
        public async Task<ActionResult> ViewProject(int projectId)
        {
            var command = new ViewProjectCommand()
            {
                ProjectId = projectId,
                UserId = _identityService.GetUserIdentity(),
                UserName = _identityService.GetUserName()
            };

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPut]
        [Route("join")]
        public async Task<ActionResult> JoinProject([FromBody]Domain.AggregatesModel.ProjectAggregate.ProjectContributor contributor)
        {
            var command = new JoinProjectCommand { Contributor = contributor };
            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
