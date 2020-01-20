using Project.Domain.Seedwork;
using System;

namespace Project.Domain.AggregatesModel.ProjectAggregate
{
    /// <summary>
    /// 项目查看者列表
    /// </summary>
    public class ProjectViewer : Entity
    {
        /// <summary>
        /// 项目Id
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }
    }
}
