using Project.Domain.Seedwork;
using System;

namespace Project.Domain.AggregatesModel.ProjectAggregate
{
    /// <summary>
    /// 项目贡献者列表
    /// </summary>
    public class ProjectContributor : Entity
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

        /// <summary>
        /// 关闭者
        /// </summary>
        public bool IsCloser { get; set; }

        /// <summary>
        /// 1:财务顾问 2:投资机构
        /// </summary>
        public int ContributorType { get; set; }
    }
}
