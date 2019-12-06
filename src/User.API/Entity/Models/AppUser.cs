using Dapper.Contrib.Extensions;
using System.Collections.Generic;

namespace User.API.Entity.Models
{
    [Table("Users")]
    public class AppUser
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        /// <summary>
        /// 用户属性列表
        /// </summary>
        public List<UserProperty> Properties { get; set; }
    }
}
