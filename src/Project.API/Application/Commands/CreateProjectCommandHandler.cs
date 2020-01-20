using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ProjectEntity = Project.Domain.AggregatesModel.ProjectAggregate.Project;
using Project.Domain.AggregatesModel.ProjectAggregate;

namespace Project.API.Application.Commands
{
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectEntity>
    {
        private IProjectRepository _projectRepository;
        public CreateProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<ProjectEntity> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            _projectRepository.Add(request.Project);
            await _projectRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return request.Project;
        }
    }
}
