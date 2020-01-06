using Dapper.Contrib.Extensions;

namespace User.API.Models
{
    /// <summary>
    /// 用户属性
    /// </summary>
    [Table("UserProperties")]
    public class UserProperty
    {
        /// <summary>
        /// 用户表外键
        /// </summary>
        [ExplicitKey]
        public int AppUserId { get; set; }

        public string Key { get; set; }

        public string Text { get; set; }

        public string Value { get; set; }
    }
}
