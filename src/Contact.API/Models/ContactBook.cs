using System.Collections.Generic;

namespace Contact.API.Models
{
    /// <summary>
    /// 联系人
    /// </summary>
    public class ContactBook
    {
        public int UserId { get; set; }

        /// <summary>
        /// 联系人列表
        /// </summary>
        public List<Contact> Contacts { get; set; }
    }
}
