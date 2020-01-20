using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Project.Domain.AggregatesModel.ProjectAggregate;
using Project.Domain.Seedwork;
using ProjectEntity = Project.Domain.AggregatesModel.ProjectAggregate.Project;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Project.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ProjectContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public ProjectRepository(ProjectContext context)
        {
            _context = context;
        }

        public ProjectEntity Add(ProjectEntity project)
        {
            if (project.IsTransient())
            {
                return _context.Projects.Add(project).Entity;
            }
            else
            {
                return project;
            }
        }

        public async Task<ProjectEntity> GetAsync(int projectId)
        {
            var project = await _context.Projects
                .Include(x => x.Properties)
                .Include(x => x.Viewers)
                .Include(x => x.Contributors)
                .Include(x => x.VisibleRule)
                .SingleOrDefaultAsync();

            return project;
        }

        public void Update(ProjectEntity project)
        {
            _context.Entry(project).State = EntityState.Modified;
        }
    }
}
