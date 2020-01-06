using Dapper.Contrib.Extensions;
using System.Collections.Generic;

namespace User.API.Models
{
    /// <summary>
    /// 用户表
    /// </summary>
    [Table("Users")]
    public class AppUser
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 用户属性列表
        /// </summary>
        public List<UserProperty> Properties { get; set; }
    }
}
