using System;

namespace User.API.Entity.Models
{
    /// <summary>
    /// 用户标签
    /// </summary>
    public class UserTage
    {
        /// <summary>
        /// 用户表外键
        /// </summary>
        public int AppUserId { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }
    }
}
