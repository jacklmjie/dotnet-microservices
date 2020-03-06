using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Project.API.Application.Commands;
using MediatR;
using Project.API.Application.Services;
using Project.API.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;

namespace Project.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;
        private readonly IRecommendService _recommendService;
        private readonly IProjectQueries _projectQueries;

        public ProjectsController(IMediator mediator,
            IIdentityService identityService,
            IRecommendService recommendService,
            IProjectQueries projectQueries)
        {
            _mediator = mediator;
            _identityService = identityService;
            _recommendService = recommendService;
            _projectQueries = projectQueries;
        }

        [HttpGet]
        [Route("")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetProjects()
        {
            var userId = _identityService.GetUserIdentity();
            var projects = await _projectQueries.GetProjectsByUserId(userId);
            return Ok(projects);
        }

        [HttpGet]
        [Route("my/{projectId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetMyProjectDetail(int projectId)
        {
            var userId = _identityService.GetUserIdentity();
            var project = await _projectQueries.GetProjectDetail(projectId);

            if (project.UserId == userId)
            {
                return Ok(project);
            }
            else
            {
                return BadRequest("无权查看该项目");
            }
        }

        [HttpGet]
        [Route("recommends/{projectId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetRecommendProjectDetail(int projectId)
        {
            var userId = _identityService.GetUserIdentity();
            if (!await _recommendService.IsProjectInRecommend(projectId, userId))
            {
                return BadRequest("不具有查看该项目的权限");
            }

            var project = await _projectQueries.GetProjectDetail(projectId);
            return Ok(project);
        }

        [HttpPost]
        [Route("")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateProject([FromBody] Domain.AggregatesModel.ProjectAggregate.Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            project.UserId = _identityService.GetUserIdentity();
            var command = new CreateProjectCommand() { Project = project };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        [Route("view")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ViewProject(int projectId)
        {
            var userId = _identityService.GetUserIdentity();
            if (!await _recommendService.IsProjectInRecommend(projectId, userId))
            {
                return BadRequest("不具有查看该项目的权限");
            }

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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> JoinProject([FromBody]Domain.AggregatesModel.ProjectAggregate.ProjectContributor contributor)
        {
            var userId = _identityService.GetUserIdentity();
            if (!await _recommendService.IsProjectInRecommend(contributor.ProjectId, userId))
            {
                return BadRequest("不具有查看该项目的权限");
            }

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
