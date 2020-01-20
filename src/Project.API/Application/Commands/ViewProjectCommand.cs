using MediatR;

namespace Project.API.Application.Commands
{
    public class ViewProjectCommand : IRequest<bool>
    {
        public int ProjectId { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }
    }
}
