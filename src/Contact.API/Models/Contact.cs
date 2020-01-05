using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contact.API.Models
{
    /// <summary>
    /// 联系人(用户信息)
    /// </summary>
    public class Contact
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 用户标签
        /// </summary>
        public List<string> Tags { get; set; }
    }
}
