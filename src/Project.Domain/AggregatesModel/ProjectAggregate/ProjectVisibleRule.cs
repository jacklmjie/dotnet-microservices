using Project.Domain.Seedwork;
using System;

namespace Project.Domain.AggregatesModel.ProjectAggregate
{
    /// <summary>
    /// 项目可见范围
    /// </summary>
    public class ProjectVisibleRule : Entity
    {
        /// <summary>
        /// 项目Id
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// 标签下的人
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }
    }
}
