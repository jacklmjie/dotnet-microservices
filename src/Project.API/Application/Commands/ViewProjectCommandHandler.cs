using MediatR;
using Project.Domain.AggregatesModel.ProjectAggregate;
using System.Threading;
using System.Threading.Tasks;

namespace Project.API.Application.Commands
{
    public class ViewProjectCommandHandler : IRequestHandler<ViewProjectCommand, bool>
    {
        private IProjectRepository _projectRepository;
        public ViewProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<bool> Handle(ViewProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetAsync(request.ProjectId);
            if (project == null)
            {
                throw new Domain.Exceptions.ProjectDomainException($"project not found {request.ProjectId}");
            }

            if (project.UserId == request.UserId)
            {
                throw new Domain.Exceptions.ProjectDomainException($"you cannot view your own project");
            }

            project.AddViewer(request.UserId, request.UserName);
            return await _projectRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
