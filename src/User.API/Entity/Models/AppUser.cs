using System.Collections.Generic;

namespace User.API.Entity.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        public int Age { get; set; }

        /// <summary>
        /// 用户属性列表
        /// </summary>
        public List<UserProperty> Properties { get; set; }
    }
}
