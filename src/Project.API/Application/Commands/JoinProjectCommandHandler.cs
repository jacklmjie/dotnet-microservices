using MediatR;
using Project.Domain.AggregatesModel.ProjectAggregate;
using System.Threading;
using System.Threading.Tasks;

namespace Project.API.Application.Commands
{
    public class JoinProjectCommandHandler : IRequestHandler<JoinProjectCommand, bool>
    {
        private IProjectRepository _projectRepository;
        public JoinProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<bool> Handle(JoinProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetAsync(request.Contributor.ProjectId);
            if (project == null)
            {
                throw new Domain.Exceptions.ProjectDomainException($"project not found {request.Contributor.ProjectId}");
            }

            project.AddContributor(request.Contributor);
            return await _projectRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
