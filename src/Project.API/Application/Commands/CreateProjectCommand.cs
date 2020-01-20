using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectEntity = Project.Domain.AggregatesModel.ProjectAggregate.Project;

namespace Project.API.Application.Commands
{
    public class CreateProjectCommand : IRequest<ProjectEntity>
    {
        //有必要可以建ViewModel传参和Dto返回值
       public ProjectEntity Project { get; set; }
    }
}
