using Project.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using Project.Domain.Events;

namespace Project.Domain.AggregatesModel.ProjectAggregate
{
    /// <summary>
    /// 项目
    /// </summary>
    public class Project : Entity, IAggregateRoot
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 可见范围
        /// </summary>
        public ProjectVisibleRule VisibleRule { get; set; }

        /// <summary>
        /// 项目标签
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 项目属性
        /// </summary>
        public List<ProjectProperty> Properties { get; set; }

        /// <summary>
        /// 贡献者
        /// </summary>
        public List<ProjectContributor> Contributors { get; set; }

        /// <summary>
        /// 查看者
        /// </summary>
        public List<ProjectViewer> Viewers { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }

        //领域行为

        public Project()
        {
            Contributors = new List<ProjectContributor>();
            Viewers = new List<ProjectViewer>();
            AddDomainEvent(new ProjectCreatedDomainEvent(this));
        }

        public void AddViewer(int userId, string userName)
        {

            ProjectViewer viewer = new ProjectViewer
            {
                UserId = userId,
                UserName = userName,
                CreatedTime = DateTime.Now
            };

            if (Viewers.Any(v => v.UserId == UserId))
            {
                Viewers.Add(viewer);

                AddDomainEvent(new ProjectViewedDomainEvent(this.Name, viewer));
            }
        }

        public void AddContributor(ProjectContributor contributor)
        {
            if (Contributors.Any(v => v.UserId == UserId))
            {
                Contributors.Add(contributor);

                AddDomainEvent(new ProjectJoinedDomainEvent(this.Name, contributor));
            }
        }
    }
}
